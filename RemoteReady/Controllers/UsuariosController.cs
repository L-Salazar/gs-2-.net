using RemoteReady.Doc.Samples;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RemoteReady.Controllers
{
    [Route("api/v1/usuario")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioUseCase _usuarioUseCase;
        private readonly IConfiguration _configuration;

        public UsuariosController(IUsuarioUseCase usuarioUseCase, IConfiguration configuration)
        {
            _usuarioUseCase = usuarioUseCase;
            _configuration = configuration;
        }

        // POST api/v1/usuario/autenticar
        [HttpPost("autenticar")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Autentica um usuário",
            Description = "Realiza a autenticação do usuário e retorna um token JWT."
        )]
        [SwaggerResponse(statusCode: 200, description: "Autenticação realizada com sucesso")]
        [SwaggerResponse(statusCode: 401, description: "Credenciais inválidas")]
        [SwaggerRequestExample(typeof(UsuarioDto), typeof(UsuarioRequestSample))]
        public async Task<IActionResult> Autenticar([FromBody] UsuarioDto dto)
        {
            var result = await _usuarioUseCase.AutenticarUserAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["Secretkey"]!.ToString());

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, result.Value!.Nome),
                    new Claim(ClaimTypes.Email, result.Value!.Email),
                    new Claim(ClaimTypes.Role, result.Value!.TipoUsuario),
                    new Claim("Id", result.Value!.Id.ToString())
                })
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return StatusCode(result.StatusCode, new
            {
                user = result.Value.Nome,
                email = result.Value.Email,
                tipoUsuario = result.Value.TipoUsuario,
                token = tokenHandler.WriteToken(token)
            });
        }

        // GET api/v1/usuario
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista usuários",
            Description = "Retorna a lista paginada de usuários cadastrados."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de usuários retornada com sucesso", type: typeof(IEnumerable<UsuarioEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há usuários cadastrados")]
        [SwaggerResponseExample(statusCode: 200, typeof(UsuarioResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> Get(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _usuarioUseCase.ObterTodosUsuariosAsync(PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens.Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.TipoUsuario,
                    u.DataCriacao,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), "Usuarios", new { id = u.Id }, Request.Scheme),
                        update = Url.Action(nameof(Put), "Usuarios", new { id = u.Id }, Request.Scheme),
                        delete = Url.Action(nameof(Delete), "Usuarios", new { id = u.Id }, Request.Scheme)
                    }
                }),
                links = new
                {
                    self = Url.Action(nameof(Get), "Usuarios", new { PaginaAtual, LimitePagina }, Request.Scheme),
                    create = Url.Action(nameof(Post), "Usuarios", null, Request.Scheme),
                    first = Url.Action(nameof(Get), "Usuarios", new { PaginaAtual = 1, LimitePagina }, Request.Scheme),
                    prev = PaginaAtual > 1
                                ? Url.Action(nameof(Get), "Usuarios", new { PaginaAtual = PaginaAtual - 1, LimitePagina }, Request.Scheme)
                                : null,
                    next = PaginaAtual < result.Value?.TotalPaginas
                                ? Url.Action(nameof(Get), "Usuarios", new { PaginaAtual = PaginaAtual + 1, LimitePagina }, Request.Scheme)
                                : null,
                    last = Url.Action(nameof(Get), "Usuarios", new { PaginaAtual = result.Value?.TotalPaginas, LimitePagina }, Request.Scheme)
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

        // GET api/v1/usuario/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtém usuário por ID",
            Description = "Retorna o usuário correspondente ao ID informado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Usuário encontrado", type: typeof(UsuarioEntity))]
        [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
        [SwaggerResponseExample(statusCode: 200, typeof(UsuarioResponseSamples))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _usuarioUseCase.ObterUmUsuarioAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "Usuarios", new { id }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "Usuarios", null, Request.Scheme),
                    update = Url.Action(nameof(Put), "Usuarios", new { id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Usuarios", new { id }, Request.Scheme),
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // POST api/v1/usuario
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Cadastra um novo usuário",
            Description = "Cria um novo usuário no sistema."
        )]
        [SwaggerRequestExample(typeof(UsuarioDto), typeof(UsuarioRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Usuário criado com sucesso", type: typeof(UsuarioEntity))]
        [SwaggerResponseExample(statusCode: 201, typeof(UsuarioResponseSamples))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> Post([FromBody] UsuarioDto dto)
        {
            var result = await _usuarioUseCase.AdicionarUsuarioAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "Usuarios", new { id = result.Value?.Id }, Request.Scheme),
                    update = Url.Action(nameof(Put), "Usuarios", new { id = result.Value?.Id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Usuarios", new { id = result.Value?.Id }, Request.Scheme),
                    login = Url.Action(nameof(Autenticar), "Usuarios", null, Request.Scheme)
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // PUT api/v1/usuario/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza um usuário",
            Description = "Atualiza os dados de um usuário existente."
        )]
        [SwaggerResponse(statusCode: 200, description: "Usuário atualizado com sucesso", type: typeof(UsuarioEntity))]
        [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
        [SwaggerRequestExample(typeof(UsuarioDto), typeof(UsuarioRequestUpdateSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDto dto)
        {
            var result = await _usuarioUseCase.EditarUsuarioAsync(id, dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }

        // DELETE api/v1/usuario/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deleta um usuário",
            Description = "Remove um usuário do sistema."
        )]
        [SwaggerResponse(statusCode: 200, description: "Usuário deletado com sucesso", type: typeof(UsuarioEntity))]
        [SwaggerResponse(statusCode: 404, description: "Usuário não encontrado")]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _usuarioUseCase.DeletarUsuarioAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }
    }
}