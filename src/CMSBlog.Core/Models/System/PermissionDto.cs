namespace CMSBlog.Core.Models.System
{
    public class PermissionDto
    {
        public string RoleId { get; set; }
        public IList<RoleClaimDto> RoleClaims { get; set; }
    }
}
