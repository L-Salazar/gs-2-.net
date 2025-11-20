using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Interfaces
{
    public interface IBlogPostUseCase
    {
        Task<OperationResult<PageData<BlogPostEntity>>> ObterTodosPostsAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<BlogPostEntity?>> ObterUmPostAsync(int Id);
        Task<OperationResult<BlogPostEntity?>> AdicionarPostAsync(BlogPostDto entity);
        Task<OperationResult<BlogPostEntity?>> EditarPostAsync(int Id, BlogPostDto entity);
        Task<OperationResult<BlogPostEntity?>> DeletarPostAsync(int Id);
        Task<OperationResult<PageData<BlogPostEntity>>> ObterPostsPorTagAsync(string tag, int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<PageData<BlogPostEntity>>> ObterPostsRecentesAsync(int PaginaAtual = 1, int LimitePagina = 10);
    }
}