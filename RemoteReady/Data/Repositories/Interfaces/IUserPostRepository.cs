using RemoteReady.Models;

namespace RemoteReady.Data.Repositories.Interfaces
{
    public interface IUserPostRepository
    {
        Task<PageData<UserPostEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<UserPostEntity?> ObterUmAsync(int Id);
        Task<UserPostEntity?> AdicionarAsync(UserPostEntity entity);
        Task<UserPostEntity?> EditarAsync(int Id, UserPostEntity entity);
        Task<UserPostEntity?> DeletarAsync(int Id);
        Task<int> ObterTotalAsync();
        Task<PageData<UserPostEntity>> ObterPorUsuarioAsync(int idUsuario, int PaginaAtual = 1, int LimitePagina = 10);
        Task<int> ObterTotalPostsLidosPorUsuarioAsync(int idUsuario);
        Task<bool> UsuarioJaLeuPostAsync(int idUsuario, int idPost);
        Task<UserPostEntity?> ObterPorUsuarioEPostAsync(int idUsuario, int idPost);
    }
}