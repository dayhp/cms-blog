using CMSBlog.Core.Domain.Identity;
using CMSBlog.Core.Models.System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Reflection;

namespace CMSBlog.API.Extensions
{
    public static class ClaimExtensions
    {
        public static void GetPermission(this List<RoleClaimDto> allPermission, Type policy)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                //var attribute = field.GetCustomAttribute(typeof(DescriptionAttribute), true);
                string displayName = field.GetValue(null)?.ToString() ?? string.Empty;
                var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attributes.Length > 0)
                {
                    var description = (DescriptionAttribute)attributes[0];
                    displayName = description.Description;
                }
                allPermission.Add(new RoleClaimDto
                {
                    Type = "Permission",
                    //Type = policy.Name,
                    Value = field.GetValue(null)?.ToString() ?? string.Empty,
                    DisplayName = displayName
                });
            }
        }

        public static async Task AddPermissionClaim(this RoleManager<AppRole> roleManager, AppRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(c => c.Type == "Permission" && c.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new System.Security.Claims.Claim("Permission", permission));
            }
        }
    }
}
