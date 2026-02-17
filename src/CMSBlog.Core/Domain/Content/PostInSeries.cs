using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("PostInSeries")]
    public class PostInSeries : BaseDomain
    {
        public Guid SeriesId { get; set; }
        public Guid PostId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
