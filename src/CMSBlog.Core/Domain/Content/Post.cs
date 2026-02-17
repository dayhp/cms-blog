using CMSBlog.Core.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMSBlog.Core.Domain.Content
{
    [Table("Posts")]
    public class Post : BaseDomain
    {
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public required string Description { get; set; }
        public Guid CategoryId { get; set; }
        public string? Thumbnail { get; set; }
        public string? Content { get; set; }
        public Guid AuthorId { get; set; }
        public string? Source { get; set; }
        public string? Tags { get; set; }
        public string? SeoDescription { get; set; }
        public int ViewCount { get; set; }
        public bool IsPublished { get; set; }
        public double RoyaltyAmount { get; set; }
        public PostStatus Status { get; set; }
    }
}
