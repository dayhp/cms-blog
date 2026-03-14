using CMSBlog.API.Extensions;
using CMSBlog.API.Services;
using CMSBlog.Core.Constant;
using CMSBlog.Core.Domain.Identity;
using CMSBlog.Core.Models.Auth;
using CMSBlog.Core.Models.System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace CMSBlog.API.Controllers.Auth
{
    [Route("api/admin/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ITokenService _tokenService;
        public AuthController(ILogger<AuthController> logger,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            RoleManager<AppRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedResult>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            if (request == null)
            {
                return BadRequest("Invalid request");
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", request.Email);
                return Unauthorized("Invalid credentials");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", request.Email);
                return Unauthorized("Invalid credentials");
            }
            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: Inactive user for email: {Email}", request.Email);
                return Unauthorized("User account is inactive");
            }
            if (user.LockoutEnabled)
            {
                _logger.LogWarning("Login failed: Locked user for email: {Email}", request.Email);
                return Unauthorized("User account is locked");
            }
            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                _logger.LogWarning("Login failed: Sign-in failed for email: {Email}", request.Email);
                return Unauthorized("Invalid credentials");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = await GetUserPermissionsAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Surname, user.FirstName),
                new Claim(ClaimTypes.Role, string.Join(",", roles)),
                new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permissions)),
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
            };
            var accessToken = await _tokenService.GenerateAccessTokenAsync(claims);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
            await _userManager.UpdateAsync(user);
            var result = new AuthenticatedResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return Ok(result);
            // Here you would generate the access token and refresh token
        }

        private async Task<List<string>> GetUserPermissionsAsync(AppUser userInput)
        {
            var user = await _userManager.FindByIdAsync(userInput.Id.ToString());
            var permissions = new List<string>();
            if (user != null)
            {
                var allPermissions = new List<RoleClaimDto>();
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(Roles.Admin))
                {
                    var types = typeof(Permissions).GetNestedTypes();
                    foreach (var type in types)
                    {
                        allPermissions.GetPermission(type);
                    }
                    permissions.AddRange(allPermissions.Select(p => p.Value).ToList());
                }
                else
                {
                    foreach (var roleName in roles)
                    {
                        var role = await _roleManager.FindByNameAsync(roleName);
                        var claims = await _roleManager.GetClaimsAsync(role);
                        var roleClaimValue = claims.Select(c => c.Value).ToList();
                        permissions.AddRange(roleClaimValue);
                    }

                }
            }
            return permissions.Distinct().ToList();
        }
    }
}
