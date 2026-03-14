namespace CMSBlog.Core.ConfigOptions
{
    public class JwtTokenSetting
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public int ExpirationInMinutes { get; set; } = 60;
    }
}
