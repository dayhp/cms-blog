using CMSBlog.Core.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("PostActivityLogs")]
    public class PostActivityLog : BaseDomain
    {
        public Guid PostId { get; set; }
        public PostStatus FromStatus { get; set; }
        public PostStatus ToStatus { get; set; }
        public string? Note { get; set; }
        public Guid UserId { get; set; }
    }
}
