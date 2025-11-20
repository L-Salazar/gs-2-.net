using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Interfaces
{
    public interface IEmpresaUseCase
    {
        Task<OperationResult<PageData<EmpresaEntity>>> ObterTodasEmpresasAsync(int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<EmpresaEntity?>> ObterUmaEmpresaAsync(int Id);
        Task<OperationResult<EmpresaEntity?>> AdicionarEmpresaAsync(EmpresaDto entity);
        Task<OperationResult<EmpresaEntity?>> EditarEmpresaAsync(int Id, EmpresaDto entity);
        Task<OperationResult<EmpresaEntity?>> DeletarEmpresaAsync(int Id);
        Task<OperationResult<PageData<EmpresaEntity>>> ObterEmpresasPorAreaAsync(string area, int PaginaAtual = 1, int LimitePagina = 10);
        Task<OperationResult<PageData<EmpresaEntity>>> ObterEmpresasContratandoAsync(int PaginaAtual = 1, int LimitePagina = 10);
    }
}