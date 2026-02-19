using AutoMapper;
using CMSBlog.Core.Domain.Content;
using CMSBlog.Core.Models;
using CMSBlog.Core.Models.Content;
using CMSBlog.Core.Repositories;
using CMSBlog.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace CMSBlog.Data.Repositories
{
    public class PostRepository : RepositoryBase<Post, Guid>, IPostRepository
    {
        private readonly IMapper _mapper;
        public PostRepository(ApplicationDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Post>> GetPopularPostAysnc(int count)
        {
            return await _context.Posts.OrderByDescending(p => p.ViewCount).Take(count).ToListAsync();
        }

        public async Task<PageResult<PostPagingDto>> GetPostsPagingAsync(string? keyword, Guid? categoryId, int page = 1, int pageSize = 10)
        {
            var query = _context.Posts.AsQueryable();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var totalCount = await query.CountAsync();
            query = query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var results = _mapper.Map<List<PostPagingDto>>(await query.ToListAsync());
            return new PageResult<PostPagingDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalCount,
                Results = results
            };
        }
    }
}
