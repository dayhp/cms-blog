using CMSBlog.Core.ConfigOptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace CMSBlog.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly JwtTokenSetting _jwtTokenSetting;
        public TokenService(ILogger<TokenService> logger, IOptions<JwtTokenSetting> jwtTokenSetting)
        {
            _logger = logger;
            _jwtTokenSetting = jwtTokenSetting.Value;
        }
        public async Task<string> GenerateAccessTokenAsync(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtTokenSetting.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtTokenSetting.Issuer,
                audience: _jwtTokenSetting.Audience,
                claims: claims,
                 expires: DateTime.UtcNow.AddMinutes(_jwtTokenSetting.ExpirationInMinutes),
                //expires: DateTime.UtcNow.AddSeconds(5),
                signingCredentials: credentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return await Task.FromResult(tokenString);
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return await Task.FromResult(Convert.ToBase64String(randomNumber));
            }
        }


        /// <summary>
        /// Get the claims principal from an expired token. This is used to validate the refresh token and generate a new access token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="SecurityTokenException"></exception>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtTokenSetting.SecretKey)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}
