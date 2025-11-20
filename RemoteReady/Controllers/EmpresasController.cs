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
    [Route("api/v1/empresa")]
    [ApiController]
    [Authorize]
    public class EmpresasController : ControllerBase
    {
        private readonly IEmpresaUseCase _empresaUseCase;

        public EmpresasController(IEmpresaUseCase empresaUseCase)
        {
            _empresaUseCase = empresaUseCase;
        }

        // GET api/v1/empresa
        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista empresas",
            Description = "Retorna a lista paginada de empresas cadastradas."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de empresas retornada com sucesso", type: typeof(IEnumerable<EmpresaEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há empresas cadastradas")]
        [SwaggerResponseExample(statusCode: 200, typeof(EmpresaResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> Get(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _empresaUseCase.ObterTodasEmpresasAsync(PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens.Select(e => new
                {
                    e.Id,
                    e.Nome,
                    e.Descricao,
                    e.Area,
                    e.ContratandoAgora,
                    e.LogoUrl,
                    e.Website,
                    e.DataCriacao,
                    links = new
                    {
                        self = Url.Action(nameof(GetById), "Empresas", new { id = e.Id }, Request.Scheme),
                        update = Url.Action(nameof(Put), "Empresas", new { id = e.Id }, Request.Scheme),
                        delete = Url.Action(nameof(Delete), "Empresas", new { id = e.Id }, Request.Scheme)
                    }
                }),
                links = new
                {
                    self = Url.Action(nameof(Get), "Empresas", new { PaginaAtual, LimitePagina }, Request.Scheme),
                    create = Url.Action(nameof(Post), "Empresas", null, Request.Scheme),
                    hiring = Url.Action(nameof(GetContratando), "Empresas", null, Request.Scheme),
                    first = Url.Action(nameof(Get), "Empresas", new { PaginaAtual = 1, LimitePagina }, Request.Scheme),
                    prev = PaginaAtual > 1
                                ? Url.Action(nameof(Get), "Empresas", new { PaginaAtual = PaginaAtual - 1, LimitePagina }, Request.Scheme)
                                : null,
                    next = PaginaAtual < result.Value?.TotalPaginas
                                ? Url.Action(nameof(Get), "Empresas", new { PaginaAtual = PaginaAtual + 1, LimitePagina }, Request.Scheme)
                                : null,
                    last = Url.Action(nameof(Get), "Empresas", new { PaginaAtual = result.Value?.TotalPaginas, LimitePagina }, Request.Scheme)
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

        // GET api/v1/empresa/{id}
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obtém empresa por ID",
            Description = "Retorna a empresa correspondente ao ID informado."
        )]
        [SwaggerResponse(statusCode: 200, description: "Empresa encontrada", type: typeof(EmpresaEntity))]
        [SwaggerResponse(statusCode: 404, description: "Empresa não encontrada")]
        [SwaggerResponseExample(statusCode: 200, typeof(EmpresaResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _empresaUseCase.ObterUmaEmpresaAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "Empresas", new { id }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "Empresas", null, Request.Scheme),
                    update = Url.Action(nameof(Put), "Empresas", new { id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Empresas", new { id }, Request.Scheme),
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // GET api/v1/empresa/area/{area}
        [HttpGet("area/{area}")]
        [SwaggerOperation(
            Summary = "Busca empresas por área",
            Description = "Retorna a lista paginada de empresas filtradas por área de atuação."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de empresas retornada com sucesso", type: typeof(IEnumerable<EmpresaEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há empresas nesta área")]
        [SwaggerResponseExample(statusCode: 200, typeof(EmpresaResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetPorArea(string area, int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _empresaUseCase.ObterEmpresasPorAreaAsync(area, PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens,
                links = new
                {
                    self = Url.Action(nameof(GetPorArea), "Empresas", new { area, PaginaAtual, LimitePagina }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "Empresas", null, Request.Scheme),
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

        // GET api/v1/empresa/contratando
        [HttpGet("contratando")]
        [SwaggerOperation(
            Summary = "Lista empresas que estão contratando",
            Description = "Retorna a lista paginada de empresas que estão com vagas abertas."
        )]
        [SwaggerResponse(statusCode: 200, description: "Lista de empresas retornada com sucesso", type: typeof(IEnumerable<EmpresaEntity>))]
        [SwaggerResponse(statusCode: 204, description: "Não há empresas contratando no momento")]
        [SwaggerResponseExample(statusCode: 200, typeof(EmpresaResponseListSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        public async Task<IActionResult> GetContratando(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var result = await _empresaUseCase.ObterEmpresasContratandoAsync(PaginaAtual, LimitePagina);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value?.Itens,
                links = new
                {
                    self = Url.Action(nameof(GetContratando), "Empresas", new { PaginaAtual, LimitePagina }, Request.Scheme),
                    getAll = Url.Action(nameof(Get), "Empresas", null, Request.Scheme),
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

        // POST api/v1/empresa
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cadastra uma nova empresa",
            Description = "Cria uma nova empresa no sistema."
        )]
        [SwaggerRequestExample(typeof(EmpresaDto), typeof(EmpresaRequestSample))]
        [SwaggerResponse(statusCode: 201, description: "Empresa criada com sucesso", type: typeof(EmpresaEntity))]
        [SwaggerResponseExample(statusCode: 201, typeof(EmpresaResponseSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] EmpresaDto dto)
        {
            var result = await _empresaUseCase.AdicionarEmpresaAsync(dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            var hateaos = new
            {
                data = result.Value,
                links = new
                {
                    self = Url.Action(nameof(GetById), "Empresas", new { id = result.Value?.Id }, Request.Scheme),
                    update = Url.Action(nameof(Put), "Empresas", new { id = result.Value?.Id }, Request.Scheme),
                    delete = Url.Action(nameof(Delete), "Empresas", new { id = result.Value?.Id }, Request.Scheme),
                }
            };

            return StatusCode(result.StatusCode, hateaos);
        }

        // PUT api/v1/empresa/{id}
        [HttpPut("{id}")]
        [SwaggerOperation(
            Summary = "Atualiza uma empresa",
            Description = "Atualiza os dados de uma empresa existente."
        )]
        [SwaggerResponse(statusCode: 200, description: "Empresa atualizada com sucesso", type: typeof(EmpresaEntity))]
        [SwaggerResponse(statusCode: 404, description: "Empresa não encontrada")]
        [SwaggerRequestExample(typeof(EmpresaDto), typeof(EmpresaRequestUpdateSample))]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Put(int id, [FromBody] EmpresaDto dto)
        {
            var result = await _empresaUseCase.EditarEmpresaAsync(id, dto);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }

        // DELETE api/v1/empresa/{id}
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Deleta uma empresa",
            Description = "Remove uma empresa do sistema."
        )]
        [SwaggerResponse(statusCode: 200, description: "Empresa deletada com sucesso", type: typeof(EmpresaEntity))]
        [SwaggerResponse(statusCode: 404, description: "Empresa não encontrada")]
        [EnableRateLimiting("rateLimitePolicy")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _empresaUseCase.DeletarEmpresaAsync(id);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode, result.Error);

            return StatusCode(result.StatusCode, result.Value);
        }
    }
}