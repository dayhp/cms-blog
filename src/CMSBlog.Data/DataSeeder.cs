using CMSBlog.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace CMSBlog.Data
{
    public class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var paswordHasher = new PasswordHasher<AppUser>();
            var rootAdminRoleId = Guid.NewGuid();
            if (!context.Roles.Any())
            {
                await context.Roles.AddAsync(new AppRole
                {
                    Id = rootAdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    DisplayName = "Administrator"
                });
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                var userId = Guid.NewGuid();
                var user = new AppUser
                {
                    Id = userId,
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "hophuday@gmail.com",
                    FirstName = "Ho Phu",
                    LastName = "Day",
                    IsActive = true,
                    Dob = new DateTime(2000, 1, 1),
                    VipStartDate = DateTime.UtcNow,
                    VipExpireDate = DateTime.UtcNow.AddYears(1),
                    Balance = 1000000,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEnabled = false,
                };
                user.PasswordHash = paswordHasher.HashPassword(user, "Admin@123$");
                await context.Users.AddAsync(user);

                await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
                {
                    RoleId = rootAdminRoleId,
                    UserId = userId
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
