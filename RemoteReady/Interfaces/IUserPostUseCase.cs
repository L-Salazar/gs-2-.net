using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Interfaces
{
    public interface IUserPostUseCase
    {
        Task<OperationResult<PageData<UserPostEntity>>> ObterTodosUserPostsAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<UserPostEntity?>> ObterUmUserPostAsync(int Id);
        Task<OperationResult<UserPostEntity?>> MarcarComoLidoAsync(int idUsuario, int idPost);
        Task<OperationResult<UserPostEntity?>> EditarUserPostAsync(int Id, UserPostDto entity);
        Task<OperationResult<UserPostEntity?>> DeletarUserPostAsync(int Id);
        Task<OperationResult<PageData<UserPostEntity>>> ObterPostsLidosPorUsuarioAsync(int idUsuario, int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<object>> ObterProgressoUsuarioAsync(int idUsuario);
        Task<OperationResult<bool>> VerificarElegibilidadeCertificadoAsync(int idUsuario);
    }
}