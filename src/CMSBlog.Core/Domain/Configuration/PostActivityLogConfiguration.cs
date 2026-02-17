using CMSBlog.Core.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMSBlog.Core.Domain.Configuration
{
    public class PostActivityLogConfiguration : IEntityTypeConfiguration<PostActivityLog>
    {
        public void Configure(EntityTypeBuilder<PostActivityLog> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.PostId).IsRequired();
            builder.Property(p => p.Note).HasMaxLength(500);
        }
    }
}
