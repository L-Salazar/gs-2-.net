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
    [Route("api/v1/userpost")]
    [ApiController]
    [Authorize]
    public class UserPostsController : ControllerBase
    {
        private readonly IUserPostUseCase _userPostUseCase;

        public UserPostsController(IUserPostUseCase userPostUseCase)
        {
            _userPostUseCase = userPostUseCase;
        }

        // GET api/v1/userpost
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista todos os registros de leitura",
            Description = "Retorna a lista paginada de registros de posts lidos."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista retornada com sucesso", type: typeof(IEnumerable<UserPostEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há registros cadastrados")]
        [SwaggerResponseExample(statusCode: 200, typeof(UserPostResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Get(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _userPostUseCase.ObterTodosUserPostsAsync(PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens.Select(up => new
                {
                    up.Id,
                    up.IdUsuario,
                    up.IdPost,
                    up.Status,
                    up.DataLeitura,
                    Usuario = up.Usuario != null ? new { up.Usuario.Nome, up.Usuario.Email } : null,
                    Post = up.Post != null ? new { up.Post.Titulo, up.Post.Tag } : null,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), "UserPosts", new { id = up.Id }, Request.Scheme),
                        update = Url.Action(nameof(Put), "UserPosts", new { id = up.Id }, Request.Scheme),
                        delete = Url.Action(nameof(Delete), "UserPosts", new { id = up.Id }, Request.Scheme)
                    }
                }),
                links = new
                {
                    self = Url.Action(nameof(Get), "UserPosts", new { PaginaAtual, LimitePagina }, Request.Scheme),
                    first = Url.Action(nameof(Get), "UserPosts", new { PaginaAtual = 1, LimitePagina }, Request.Scheme),
                    prev = PaginaAtual > 1
                                ? Url.Action(nameof(Get), "UserPosts", new { PaginaAtual = PaginaAtual - 1, LimitePagina }, Request.Scheme)
                                : null,
                    next = PaginaAtual < result.Value?.TotalPaginas
                                ? Url.Action(nameof(Get), "UserPosts", new { PaginaAtual = PaginaAtual + 1, LimitePagina }, Request.Scheme)
                                : null,
                    last = Url.Action(nameof(Get), "UserPosts", new { PaginaAtual = result.Value?.TotalPaginas, LimitePagina }, Request.Scheme)
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

        // GET api/v1/userpost/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtém registro por ID",
            Description = "Retorna o registro correspondente ao ID informado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Registro encontrado", type: typeof(UserPostEntity))]
        [SwaggerResponse(statusCode: 404, description: "Registro não encontrado")]
        [SwaggerResponseExample(statusCode: 200, typeof(UserPostResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userPostUseCase.ObterUmUserPostAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "UserPosts", new { id }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "UserPosts", null, Request.Scheme),
                    update = Url.Action(nameof(Put), "UserPosts", new { id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "UserPosts", new { id }, Request.Scheme),
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // GET api/v1/userpost/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        [SwaggerOperation(
            Summary = "Lista posts lidos por um usuário",
            Description = "Retorna a lista paginada de posts que um usuário já leu."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista retornada com sucesso", type: typeof(IEnumerable<UserPostEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Usuário ainda não leu nenhum post")]
        [SwaggerResponseExample(statusCode: 200, typeof(UserPostResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetPorUsuario(int idUsuario, int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _userPostUseCase.ObterPostsLidosPorUsuarioAsync(idUsuario, PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens,
                links = new
                {
                    self = Url.Action(nameof(GetPorUsuario), "UserPosts", new { idUsuario, PaginaAtual, LimitePagina }, Request.Scheme),
                    progresso = Url.Action(nameof(GetProgresso), "UserPosts", new { idUsuario }, Request.Scheme),
                    certificado = Url.Action(nameof(VerificarCertificado), "UserPosts", new { idUsuario }, Request.Scheme)
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

        // GET api/v1/userpost/progresso/{idUsuario}
        [HttpGet("progresso/{idUsuario}")]
        [SwaggerOperation(
            Summary = "Obtém progresso de leitura do usuário",
            Description = "Retorna estatísticas de progresso incluindo total de posts lidos e elegibilidade para certificado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Progresso retornado com sucesso")]
        [SwaggerResponseExample(statusCode: 200, typeof(ProgressoUsuarioResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetProgresso(int idUsuario)
        {
            var result = await _userPostUseCase.ObterProgressoUsuarioAsync(idUsuario);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }

        // GET api/v1/userpost/certificado/{idUsuario}
        [HttpGet("certificado/{idUsuario}")]
        [SwaggerOperation(
            Summary = "Verifica elegibilidade para certificado",
            Description = "Verifica se o usuário já leu 10 ou mais posts e está elegível para gerar certificado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Elegibilidade verificada")]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> VerificarCertificado(int idUsuario)
        {
            var result = await _userPostUseCase.VerificarElegibilidadeCertificadoAsync(idUsuario);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var response = new
            {
                IdUsuario = idUsuario,
                ElegivelParaCertificado = result.Value,
                Mensagem = result.Value == true
                    ? "Parabéns! Você está elegível para gerar seu certificado."
                    : "Você ainda não completou os requisitos para o certificado. Continue lendo!",
                links = new
                {
                    progresso = Url.Action(nameof(GetProgresso), "UserPosts", new { idUsuario }, Request.Scheme),
                    postsLidos = Url.Action(nameof(GetPorUsuario), "UserPosts", new { idUsuario }, Request.Scheme)
                }
            };

            return StatusCode(result.StatusCode, response);
        }

        // POST api/v1/userpost/marcar-lido
        [HttpPost("marcar-lido")]
        [SwaggerOperation(
            Summary = "Marca um post como lido",
            Description = "Registra que o usuário leu um post específico. Usado para tracking e gamificação."
        )]
        [SwaggerResponse(statusCode: 201, description: "Post marcado como lido com sucesso", type: typeof(UserPostEntity))]
        [SwaggerResponse(statusCode: 400, description: "Post já foi marcado como lido anteriormente")]
        [SwaggerResponseExample(statusCode: 201, typeof(UserPostResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> MarcarComoLido([FromQuery] int idUsuario, [FromQuery] int idPost)
        {
            var result = await _userPostUseCase.MarcarComoLidoAsync(idUsuario, idPost);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "UserPosts", new { id = result.Value?.Id }, Request.Scheme),
                    progresso = Url.Action(nameof(GetProgresso), "UserPosts", new { idUsuario }, Request.Scheme),
                    postsLidos = Url.Action(nameof(GetPorUsuario), "UserPosts", new { idUsuario }, Request.Scheme)
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // PUT api/v1/userpost/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um registro",
            Description = "Atualiza o status de um registro de leitura."
        )]
        [SwaggerResponse(statusCode: 200, description: "Registro atualizado com sucesso", type: typeof(UserPostEntity))]
        [SwaggerResponse(statusCode: 404, description: "Registro não encontrado")]
        [SwaggerRequestExample(typeof(UserPostDto), typeof(UserPostRequestUpdateSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] UserPostDto dto)
        {
            var result = await _userPostUseCase.EditarUserPostAsync(id, dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }

        // DELETE api/v1/userpost/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deleta um registro",
            Description = "Remove um registro de leitura do sistema."
        )]
        [SwaggerResponse(statusCode: 200, description: "Registro deletado com sucesso", type: typeof(UserPostEntity))]
        [SwaggerResponse(statusCode: 404, description: "Registro não encontrado")]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userPostUseCase.DeletarUserPostAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }
    }
}