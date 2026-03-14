using System.Security.Claims;

namespace CMSBlog.API.Services
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(IEnumerable<Claim> claims);
        Task<string> GenerateRefreshTokenAsync();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
