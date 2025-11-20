using RemoteReady.Doc.Samples;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace RemoteReady.Controllers
{
    [Route("api/v1/blogpost")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostUseCase _blogPostUseCase;

        public BlogPostsController(IBlogPostUseCase blogPostUseCase)
        {
            _blogPostUseCase = blogPostUseCase;
        }

        // GET api/v1/blogpost
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista posts do blog",
            Description = "Retorna a lista paginada de posts cadastrados, ordenados do mais recente para o mais antigo."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de posts retornada com sucesso", type: typeof(IEnumerable<BlogPostEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há posts cadastrados")]
        [SwaggerResponseExample(statusCode: 200, typeof(BlogPostResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _blogPostUseCase.ObterTodosPostsAsync(PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens.Select(p => new
                {
                    p.Id,
                    p.Titulo,
                    p.Descricao,
                    p.ImageUrl,
                    p.Tag,
                    p.DataCriacao,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), "BlogPosts", new { id = p.Id }, Request.Scheme),
                        update = Url.Action(nameof(Put), "BlogPosts", new { id = p.Id }, Request.Scheme),
                        delete = Url.Action(nameof(Delete), "BlogPosts", new { id = p.Id }, Request.Scheme)
                    }
                }),
                links = new
                {
                    self = Url.Action(nameof(Get), "BlogPosts", new { PaginaAtual, LimitePagina }, Request.Scheme),
                    create = Url.Action(nameof(Post), "BlogPosts", null, Request.Scheme),
                    recentes = Url.Action(nameof(GetRecentes), "BlogPosts", null, Request.Scheme),
                    first = Url.Action(nameof(Get), "BlogPosts", new { PaginaAtual = 1, LimitePagina }, Request.Scheme),
                    prev = PaginaAtual > 1
                                ? Url.Action(nameof(Get), "BlogPosts", new { PaginaAtual = PaginaAtual - 1, LimitePagina }, Request.Scheme)
                                : null,
                    next = PaginaAtual < result.Value?.TotalPaginas
                                ? Url.Action(nameof(Get), "BlogPosts", new { PaginaAtual = PaginaAtual + 1, LimitePagina }, Request.Scheme)
                                : null,
                    last = Url.Action(nameof(Get), "BlogPosts", new { PaginaAtual = result.Value?.TotalPaginas, LimitePagina }, Request.Scheme)
                },
                pagina = new
                {
                    PaginaAtual = result.Value?.PaginaAtual,
                    TotalPaginas = result.Value?.TotalPaginas,
                    TotalRegistros = result.Value?.TotalRegistros
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // GET api/v1/blogpost/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtém post por ID",
            Description = "Retorna o post correspondente ao ID informado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Post encontrado", type: typeof(BlogPostEntity))]
        [SwaggerResponse(statusCode: 404, description: "Post não encontrado")]
        [SwaggerResponseExample(statusCode: 200, typeof(BlogPostResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _blogPostUseCase.ObterUmPostAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "BlogPosts", new { id }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "BlogPosts", null, Request.Scheme),
                    update = Url.Action(nameof(Put), "BlogPosts", new { id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "BlogPosts", new { id }, Request.Scheme),
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // GET api/v1/blogpost/tag/{tag}
        [HttpGet("tag/{tag}")]
        [SwaggerOperation(
            Summary = "Busca posts por tag",
            Description = "Retorna a lista paginada de posts filtrados por tag."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de posts retornada com sucesso", type: typeof(IEnumerable<BlogPostEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há posts com esta tag")]
        [SwaggerResponseExample(statusCode: 200, typeof(BlogPostResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPorTag(string tag, int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _blogPostUseCase.ObterPostsPorTagAsync(tag, PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens,
                links = new
                {
                    self = Url.Action(nameof(GetPorTag), "BlogPosts", new { tag, PaginaAtual, LimitePagina }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "BlogPosts", null, Request.Scheme),
                },
                pagina = new
                {
                    PaginaAtual = result.Value?.PaginaAtual,
                    TotalPaginas = result.Value?.TotalPaginas,
                    TotalRegistros = result.Value?.TotalRegistros
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // GET api/v1/blogpost/recentes
        [HttpGet("recentes")]
        [SwaggerOperation(
            Summary = "Lista posts mais recentes",
            Description = "Retorna a lista paginada de posts ordenados por data de criação (mais recentes primeiro)."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de posts retornada com sucesso", type: typeof(IEnumerable<BlogPostEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há posts cadastrados")]
        [SwaggerResponseExample(statusCode: 200, typeof(BlogPostResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecentes(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _blogPostUseCase.ObterPostsRecentesAsync(PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens,
                links = new
                {
                    self = Url.Action(nameof(GetRecentes), "BlogPosts", new { PaginaAtual, LimitePagina }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "BlogPosts", null, Request.Scheme),
                },
                pagina = new
                {
                    PaginaAtual = result.Value?.PaginaAtual,
                    TotalPaginas = result.Value?.TotalPaginas,
                    TotalRegistros = result.Value?.TotalRegistros
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // POST api/v1/blogpost
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra um novo post",
            Description = "Cria um novo post no blog."
        )]
        [SwaggerRequestExample(typeof(BlogPostDto), typeof(BlogPostRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Post criado com sucesso", type: typeof(BlogPostEntity))]
        [SwaggerResponseExample(statusCode: 201, typeof(BlogPostResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] BlogPostDto dto)
        {
            var result = await _blogPostUseCase.AdicionarPostAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "BlogPosts", new { id = result.Value?.Id }, Request.Scheme),
                    update = Url.Action(nameof(Put), "BlogPosts", new { id = result.Value?.Id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "BlogPosts", new { id = result.Value?.Id }, Request.Scheme),
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // PUT api/v1/blogpost/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um post",
            Description = "Atualiza os dados de um post existente."
        )]
        [SwaggerResponse(statusCode: 200, description: "Post atualizado com sucesso", type: typeof(BlogPostEntity))]
        [SwaggerResponse(statusCode: 404, description: "Post não encontrado")]
        [SwaggerRequestExample(typeof(BlogPostDto), typeof(BlogPostRequestUpdateSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] BlogPostDto dto)
        {
            var result = await _blogPostUseCase.EditarPostAsync(id, dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }

        // DELETE api/v1/blogpost/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deleta um post",
            Description = "Remove um post do blog."
        )]
        [SwaggerResponse(statusCode: 200, description: "Post deletado com sucesso", type: typeof(BlogPostEntity))]
        [SwaggerResponse(statusCode: 404, description: "Post não encontrado")]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _blogPostUseCase.DeletarPostAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }
    }
}