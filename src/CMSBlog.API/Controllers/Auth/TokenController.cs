using CMSBlog.API.Services;
using CMSBlog.Core.Domain.Identity;
using CMSBlog.Core.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CMSBlog.API.Controllers.Auth
{
    [Route("api/admin/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        public TokenController(
            ILogger<TokenController> logger,
            UserManager<AppUser> userManager,
            ITokenService tokenService)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<AuthenticatedResult>> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (tokenRequest == null)
            {
                return BadRequest("Invalid request");
            }
            string accessToken = tokenRequest.AccessToken;
            string refreshToken = tokenRequest.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null || principal.Identity == null || principal.Identity.Name == null)
            {
                return BadRequest("Invalid token");
            }
            string username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid client request");
            }
            var newAccessToken = await _tokenService.GenerateAccessTokenAsync(principal.Claims);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return Ok(new AuthenticatedResult
            {
                AccessToken = newAccessToken,
                RefreshToken = user.RefreshToken
            });
        }

        [HttpPost, Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
            var username = User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Invalid client request");
            }
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return BadRequest("User is null");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}