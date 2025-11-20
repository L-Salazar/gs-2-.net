using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Interfaces
{
    public interface IUsuarioUseCase
    {
        Task<OperationResult<PageData<UsuarioEntity>>> ObterTodosUsuariosAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<UsuarioEntity?>> ObterUmUsuarioAsync(int Id);
        Task<OperationResult<UsuarioEntity?>> AdicionarUsuarioAsync(UsuarioDto entity);
        Task<OperationResult<UsuarioEntity?>> EditarUsuarioAsync(int Id, UsuarioDto entity);
        Task<OperationResult<UsuarioEntity?>> DeletarUsuarioAsync(int Id);
        Task<OperationResult<UsuarioEntity?>> AutenticarUserAsync(UsuarioDto entity);
    }
}