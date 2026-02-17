using CMSBlog.Core.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMSBlog.Core.Domain.Configuration
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Source).HasMaxLength(128);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Thumbnail).HasMaxLength(500);
            builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Tags).HasMaxLength(250);
            builder.Property(p => p.CategoryId).IsRequired();
            builder.Property(p => p.AuthorId).IsRequired();
            builder.HasIndex(p => p.Slug).IsUnique();
        }
    }
}
