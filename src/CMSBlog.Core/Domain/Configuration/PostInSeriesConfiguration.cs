using CMSBlog.Core.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CMSBlog.Core.Domain.Configuration
{
    public class PostInSeriesConfiguration : IEntityTypeConfiguration<PostInSeries>
    {
        public void Configure(EntityTypeBuilder<PostInSeries> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasKey(p => new { p.SeriesId, p.PostId });
        }
    }
}
