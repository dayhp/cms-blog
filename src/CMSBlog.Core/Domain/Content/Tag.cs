using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("Tags")]
    public class Tag : BaseDomain
    {
        public required string Name { get; set; }
    }
}
