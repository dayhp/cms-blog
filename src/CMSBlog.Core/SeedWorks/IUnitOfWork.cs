using CMSBlog.Core.Repositories;

namespace CMSBlog.Core.SeedWorks
{
    public interface IUnitOfWork
    {
        IPostRepository PostRepository { get; }
        Task<int> CompleteAsync();
    }
}
