using CMSBlog.Core.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMSBlog.Core.Domain.Configuration
{
    public class SeriesConfiguration : IEntityTypeConfiguration<Series>
    {
        public void Configure(EntityTypeBuilder<Series> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Slug).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.HasIndex(p => p.Slug).IsUnique();
        }
    }
}
