using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Mappers;
using RemoteReady.Models;
using System.Net;

namespace RemoteReady.UseCases
{
    public class UsuarioUseCase : IUsuarioUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioUseCase(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<OperationResult<UsuarioEntity?>> AdicionarUsuarioAsync(UsuarioDto entity)
        {
            try
            {
                var usuarioEntity = entity.ToUsuarioEntity();
                var result = await _usuarioRepository.AdicionarAsync(usuarioEntity);

                return OperationResult<UsuarioEntity?>.Success(result, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UsuarioEntity?>.Failure("Não foi possível salvar o usuário", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<UsuarioEntity?>> DeletarUsuarioAsync(int Id)
        {
            try
            {
                var result = await _usuarioRepository.DeletarAsync(Id);

                if (result is null)
                    return OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<UsuarioEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UsuarioEntity?>.Failure("Não foi possível deletar o usuário", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<UsuarioEntity?>> EditarUsuarioAsync(int Id, UsuarioDto entity)
        {
            try
            {
                var usuarioEntity = entity.ToUsuarioEntity();
                var result = await _usuarioRepository.EditarAsync(Id, usuarioEntity);

                if (result is null)
                    return OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<UsuarioEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UsuarioEntity?>.Failure("Não foi possível editar o usuário", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<PageData<UsuarioEntity>>> ObterTodosUsuariosAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _usuarioRepository.ObterTodosAsync(PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<UsuarioEntity>>.Failure("Não há usuários cadastrados", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<UsuarioEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<UsuarioEntity>>.Failure("Ocorreu um erro ao obter os usuários", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<UsuarioEntity?>> ObterUmUsuarioAsync(int Id)
        {
            try
            {
                var result = await _usuarioRepository.ObterUmAsync(Id);

                if (result is null)
                    return OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<UsuarioEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao obter o usuário", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<UsuarioEntity?>> AutenticarUserAsync(UsuarioDto entity)
        {
            try
            {
                var usuario = await _usuarioRepository.ObterPorEmailAsync(entity.Email);

                if (usuario is null)
                    return OperationResult<UsuarioEntity?>.Failure("Usuário ou senha inválidos", (int)HttpStatusCode.Unauthorized);

                // Verifica a senha (comparação direta)
                if (usuario.Senha != entity.Senha)
                    return OperationResult<UsuarioEntity?>.Failure("Usuário ou senha inválidos", (int)HttpStatusCode.Unauthorized);

                return OperationResult<UsuarioEntity?>.Success(usuario);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UsuarioEntity?>.Failure("Ocorreu um erro ao autenticar o usuário", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}