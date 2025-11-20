using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Mappers;
using RemoteReady.Models;
using System.Net;

namespace RemoteReady.UseCases
{
    public class UserPostUseCase : IUserPostUseCase
    {
        private readonly IUserPostRepository _userPostRepository;
        private const int POSTS_PARA_CERTIFICADO = 10;

        public UserPostUseCase(IUserPostRepository userPostRepository)
        {
            _userPostRepository = userPostRepository;
        }

        public async Task<OperationResult<UserPostEntity?>> MarcarComoLidoAsync(int idUsuario, int idPost)
        {
            try
            {
                // Verifica se o usuário já leu este post
                var jaLeu = await _userPostRepository.UsuarioJaLeuPostAsync(idUsuario, idPost);

                if (jaLeu)
                    return OperationResult<UserPostEntity?>.Failure("Este post já foi marcado como lido", (int)HttpStatusCode.BadRequest);

                var userPostEntity = new UserPostEntity
                {
                    IdUsuario = idUsuario,
                    IdPost = idPost,
                    Status = "LIDO",
                    DataLeitura = DateTime.Now
                };

                var result = await _userPostRepository.AdicionarAsync(userPostEntity);

                return OperationResult<UserPostEntity?>.Success(result, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return OperationResult<UserPostEntity?>.Failure("Não foi possível marcar o post como lido", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<UserPostEntity?>> DeletarUserPostAsync(int Id)
        {
            try
            {
                var result = await _userPostRepository.DeletarAsync(Id);

                if (result is null)
                    return OperationResult<UserPostEntity?>.Failure("Registro não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<UserPostEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UserPostEntity?>.Failure("Não foi possível deletar o registro", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<UserPostEntity?>> EditarUserPostAsync(int Id, UserPostDto entity)
        {
            try
            {
                var userPostEntity = entity.ToUserPostEntity();
                var result = await _userPostRepository.EditarAsync(Id, userPostEntity);

                if (result is null)
                    return OperationResult<UserPostEntity?>.Failure("Registro não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<UserPostEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UserPostEntity?>.Failure("Não foi possível editar o registro", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<PageData<UserPostEntity>>> ObterTodosUserPostsAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _userPostRepository.ObterTodosAsync(PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<UserPostEntity>>.Failure("Não há registros cadastrados", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<UserPostEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<UserPostEntity>>.Failure("Ocorreu um erro ao obter os registros", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<UserPostEntity?>> ObterUmUserPostAsync(int Id)
        {
            try
            {
                var result = await _userPostRepository.ObterUmAsync(Id);

                if (result is null)
                    return OperationResult<UserPostEntity?>.Failure("Registro não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<UserPostEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<UserPostEntity?>.Failure("Ocorreu um erro ao obter o registro", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PageData<UserPostEntity>>> ObterPostsLidosPorUsuarioAsync(int idUsuario, int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _userPostRepository.ObterPorUsuarioAsync(idUsuario, PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<UserPostEntity>>.Failure("Este usuário ainda não leu nenhum post", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<UserPostEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<UserPostEntity>>.Failure("Ocorreu um erro ao obter os posts lidos", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<object>> ObterProgressoUsuarioAsync(int idUsuario)
        {
            try
            {
                var totalPostsLidos = await _userPostRepository.ObterTotalPostsLidosPorUsuarioAsync(idUsuario);
                var elegivel = totalPostsLidos >= POSTS_PARA_CERTIFICADO;
                var faltam = elegivel ? 0 : POSTS_PARA_CERTIFICADO - totalPostsLidos;

                var progresso = new
                {
                    IdUsuario = idUsuario,
                    TotalPostsLidos = totalPostsLidos,
                    PostsNecessariosParaCertificado = POSTS_PARA_CERTIFICADO,
                    PostsFaltantes = faltam,
                    ElegivelParaCertificado = elegivel,
                    PercentualConcluido = (totalPostsLidos * 100.0) / POSTS_PARA_CERTIFICADO,
                    Mensagem = elegivel
                        ? "Parabéns! Você já pode gerar seu certificado de conclusão!"
                        : $"Continue lendo! Faltam apenas {faltam} posts para você gerar seu certificado."
                };

                return OperationResult<object>.Success(progresso);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<object>.Failure("Ocorreu um erro ao obter o progresso do usuário", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<bool>> VerificarElegibilidadeCertificadoAsync(int idUsuario)
        {
            try
            {
                var totalPostsLidos = await _userPostRepository.ObterTotalPostsLidosPorUsuarioAsync(idUsuario);
                var elegivel = totalPostsLidos >= POSTS_PARA_CERTIFICADO;

                return OperationResult<bool>.Success(elegivel);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<bool>.Failure("Ocorreu um erro ao verificar elegibilidade", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}