using RemoteReady.Models;

namespace RemoteReady.Data.Repositories.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<PageData<EmpresaEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<EmpresaEntity?> ObterUmaAsync(int Id);
        Task<EmpresaEntity?> AdicionarAsync(EmpresaEntity entity);
        Task<EmpresaEntity?> EditarAsync(int Id, EmpresaEntity entity);
        Task<EmpresaEntity?> DeletarAsync(int Id);
        Task<int> ObterTotalAsync();
        Task<PageData<EmpresaEntity>> ObterPorAreaAsync(string area, int PaginaAtual = 1, int LimitePagina = 10);
        Task<PageData<EmpresaEntity>> ObterContratandoAsync(int PaginaAtual = 1, int LimitePagina = 10);
    }
}