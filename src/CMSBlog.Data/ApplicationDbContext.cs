using CMSBlog.Core.Domain.Configuration;
using CMSBlog.Core.Domain.Content;
using CMSBlog.Core.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMSBlog.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostActivityLog> PostActivityLogs { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<PostInSeries> PostInSeries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims").HasKey(x => x.Id);
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims").HasKey(x => x.Id);
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);
            builder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
            builder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);

            builder.ApplyConfiguration<Post>(new PostConfiguration());
            builder.ApplyConfiguration<PostCategory>(new PostCategoryConfiguration());
            builder.ApplyConfiguration<PostTag>(new PostTagConfiguration());
            builder.ApplyConfiguration<Tag>(new TagConfiguration());
            builder.ApplyConfiguration<PostActivityLog>(new PostActivityLogConfiguration());
            builder.ApplyConfiguration<Series>(new SeriesConfiguration());
            builder.ApplyConfiguration<PostInSeries>(new PostInSeriesConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entityEntry in entries)
            {
                var dateCreatedProp = entityEntry.Entity.GetType().GetProperty("CreatedAt");
                if (entityEntry.State == EntityState.Added && dateCreatedProp != null)
                {
                    dateCreatedProp.SetValue(entityEntry.Entity, DateTime.UtcNow);
                }

                var dateUpdatedProp = entityEntry.Entity.GetType().GetProperty("UpdatedAt");
                if (entityEntry.State == EntityState.Modified && dateUpdatedProp != null)
                {
                    dateUpdatedProp.SetValue(entityEntry.Entity, DateTime.UtcNow);
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
