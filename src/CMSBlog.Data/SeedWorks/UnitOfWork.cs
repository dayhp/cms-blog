using AutoMapper;
using CMSBlog.Core.Repositories;
using CMSBlog.Core.SeedWorks;
using CMSBlog.Data.Repositories;

namespace CMSBlog.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            PostRepository = new PostRepository(_context, mapper);
        }

        public IPostRepository PostRepository { get; private set; }

        public Task<int> CompleteAsync()
        {
            return _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
