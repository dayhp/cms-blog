using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("PostTags")]
    public class PostTag : BaseDomain
    {
        public Guid PostId { get; set; }
        public Guid TagId { get; set; }
    }
}
