using CMSBlog.Core.Domain.Content;
using CMSBlog.Core.Models;
using CMSBlog.Core.Models.Content;
using CMSBlog.Core.SeedWorks;

namespace CMSBlog.Core.Repositories
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
        Task<IEnumerable<Post>> GetPopularPostAysnc(int count);

        Task<PageResult<PostPagingDto>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int page = 1, int pageSize = 10);
    }
}
