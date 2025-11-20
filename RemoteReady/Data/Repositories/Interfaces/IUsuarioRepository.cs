using RemoteReady.Models;

namespace RemoteReady.Data.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<PageData<UsuarioEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<UsuarioEntity?> ObterUmAsync(int Id);
        Task<UsuarioEntity?> ObterPorEmailAsync(string email);
        Task<UsuarioEntity?> AdicionarAsync(UsuarioEntity entity);
        Task<UsuarioEntity?> EditarAsync(int Id, UsuarioEntity entity);
        Task<UsuarioEntity?> DeletarAsync(int Id);
        Task<int> ObterTotalAsync();
    }
}