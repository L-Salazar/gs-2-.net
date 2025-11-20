using RemoteReady.Models;

namespace RemoteReady.Data.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<PageData<BlogPostEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<BlogPostEntity?> ObterUmAsync(int Id);
        Task<BlogPostEntity?> AdicionarAsync(BlogPostEntity entity);
        Task<BlogPostEntity?> EditarAsync(int Id, BlogPostEntity entity);
        Task<BlogPostEntity?> DeletarAsync(int Id);
        Task<int> ObterTotalAsync();
        Task<PageData<BlogPostEntity>> ObterPorTagAsync(string tag, int PaginaAtual = 1, int LimitePagina = 10);
        Task<PageData<BlogPostEntity>> ObterRecentesAsync(int PaginaAtual = 1, int LimitePagina = 10);
    }
}