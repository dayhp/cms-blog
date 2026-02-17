using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("PostCategories")]
    public class PostCategory : BaseDomain
    {
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsActive { get; set; }
        public string? SeoDescription { get; set; }
        public int SortOrder { get; set; }
    }
}
