using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Mappers;
using RemoteReady.Models;
using System.Net;

namespace RemoteReady.UseCases
{
    public class BlogPostUseCase : IBlogPostUseCase
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogPostUseCase(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<OperationResult<BlogPostEntity?>> AdicionarPostAsync(BlogPostDto entity)
        {
            try
            {
                var blogPostEntity = entity.ToBlogPostEntity();
                var result = await _blogPostRepository.AdicionarAsync(blogPostEntity);

                return OperationResult<BlogPostEntity?>.Success(result, (int)HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<BlogPostEntity?>.Failure("Não foi possível salvar o post", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<BlogPostEntity?>> DeletarPostAsync(int Id)
        {
            try
            {
                var result = await _blogPostRepository.DeletarAsync(Id);

                if (result is null)
                    return OperationResult<BlogPostEntity?>.Failure("Post não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<BlogPostEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<BlogPostEntity?>.Failure("Não foi possível deletar o post", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<BlogPostEntity?>> EditarPostAsync(int Id, BlogPostDto entity)
        {
            try
            {
                var blogPostEntity = entity.ToBlogPostEntity();
                var result = await _blogPostRepository.EditarAsync(Id, blogPostEntity);

                if (result is null)
                    return OperationResult<BlogPostEntity?>.Failure("Post não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<BlogPostEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<BlogPostEntity?>.Failure("Não foi possível editar o post", (int)HttpStatusCode.BadRequest);
            }
        }

        public async Task<OperationResult<PageData<BlogPostEntity>>> ObterTodosPostsAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _blogPostRepository.ObterTodosAsync(PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<BlogPostEntity>>.Failure("Não há posts cadastrados", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<BlogPostEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<BlogPostEntity>>.Failure("Ocorreu um erro ao obter os posts", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<BlogPostEntity?>> ObterUmPostAsync(int Id)
        {
            try
            {
                var result = await _blogPostRepository.ObterUmAsync(Id);

                if (result is null)
                    return OperationResult<BlogPostEntity?>.Failure("Post não encontrado", (int)HttpStatusCode.NotFound);

                return OperationResult<BlogPostEntity?>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<BlogPostEntity?>.Failure("Ocorreu um erro ao obter o post", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PageData<BlogPostEntity>>> ObterPostsPorTagAsync(string tag, int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _blogPostRepository.ObterPorTagAsync(tag, PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<BlogPostEntity>>.Failure($"Não há posts com a tag '{tag}'", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<BlogPostEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<BlogPostEntity>>.Failure("Ocorreu um erro ao obter os posts por tag", (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<OperationResult<PageData<BlogPostEntity>>> ObterPostsRecentesAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            try
            {
                var result = await _blogPostRepository.ObterRecentesAsync(PaginaAtual, LimitePagina);

                if (!result.Itens.Any())
                    return OperationResult<PageData<BlogPostEntity>>.Failure("Não há posts cadastrados", (int)HttpStatusCode.NoContent);

                return OperationResult<PageData<BlogPostEntity>>.Success(result);
            }
            catch (Exception ex)
            {
                //Log
                return OperationResult<PageData<BlogPostEntity>>.Failure("Ocorreu um erro ao obter os posts recentes", (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}