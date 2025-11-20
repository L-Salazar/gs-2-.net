using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Mappers;
using RemoteReady.Models;
using System.Net;

namespace RemoteReady.UseCases
{
    public class EmpresaUseCase : IEmpresaUseCase
    {
        private readonly IEmpresaRepository _empresaRepository;

        public EmpresaUseCase(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public async Task<OperationResult<EmpresaEntity?>> AdicionarEmpresaAsync(EmpresaDto entity)
        {
            try
            {
                var empresaEntity = entity.ToEmpresaEntity();
                var result = await _empresaRepository.AdicionarAsync(empresaEntity);

                return OperationResult<EmpresaEntity?>.Success(result, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<EmpresaEntity?>.Failure("Não foi possível salvar a empresa", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<EmpresaEntity?>> DeletarEmpresaAsync(int Id)
        {
            try
            {
                var result = await _empresaRepository.DeletarAsync(Id);

                if (result is null)
                    return OperationResult<EmpresaEntity?>.Failure("Empresa não encontrada", (int)HttpStatusCode.NotFound);

                return OperationResult<EmpresaEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<EmpresaEntity?>.Failure("Não foi possível deletar a empresa", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<EmpresaEntity?>> EditarEmpresaAsync(int Id, EmpresaDto entity)
        {
            try
            {
                var empresaEntity = entity.ToEmpresaEntity();
                var result = await _empresaRepository.EditarAsync(Id, empresaEntity);

                if (result is null)
                    return OperationResult<EmpresaEntity?>.Failure("Empresa não encontrada", (int)HttpStatusCode.NotFound);

                return OperationResult<EmpresaEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<EmpresaEntity?>.Failure("Não foi possível editar a empresa", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<PageData<EmpresaEntity>>> ObterTodasEmpresasAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _empresaRepository.ObterTodosAsync(PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<EmpresaEntity>>.Failure("Não há empresas cadastradas", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<EmpresaEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<EmpresaEntity>>.Failure("Ocorreu um erro ao obter as empresas", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<EmpresaEntity?>> ObterUmaEmpresaAsync(int Id)
        {
            try
            {
                var result = await _empresaRepository.ObterUmaAsync(Id);

                if (result is null)
                    return OperationResult<EmpresaEntity?>.Failure("Empresa não encontrada", (int)HttpStatusCode.NotFound);

                return OperationResult<EmpresaEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<EmpresaEntity?>.Failure("Ocorreu um erro ao obter a empresa", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PageData<EmpresaEntity>>> ObterEmpresasPorAreaAsync(string area, int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _empresaRepository.ObterPorAreaAsync(area, PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<EmpresaEntity>>.Failure($"Não há empresas cadastradas na área '{area}'", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<EmpresaEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<EmpresaEntity>>.Failure("Ocorreu um erro ao obter as empresas por área", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PageData<EmpresaEntity>>> ObterEmpresasContratandoAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _empresaRepository.ObterContratandoAsync(PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<EmpresaEntity>>.Failure("Não há empresas contratando no momento", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<EmpresaEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<EmpresaEntity>>.Failure("Ocorreu um erro ao obter as empresas que estão contratando", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}