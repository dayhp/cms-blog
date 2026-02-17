using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("Series")]
    public class Series : BaseDomain
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Slug { get; set; }
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public string? SeoDescription { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public Guid AuthorUserId { get; set; }
    }
}
