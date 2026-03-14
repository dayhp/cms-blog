namespace CMSBlog.Core.Models.Auth
{
    public class AuthenticatedResult
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
