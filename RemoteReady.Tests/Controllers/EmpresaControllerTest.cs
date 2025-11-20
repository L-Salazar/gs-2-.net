using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RemoteReady.Interfaces;
using RemoteReady.Models;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Xunit;

namespace RemoteReady.Tests.Controllers
{
    // Handler de autenticação fake
    public class EmpresaTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public EmpresaTestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "test-user"),
                new Claim(ClaimTypes.Role, "ADMIN")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    // Policy evaluator fake pra ignorar autorização real
    public class EmpresaFakePolicyEvaluator : IPolicyEvaluator
    {
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "test-user"),
                new Claim(ClaimTypes.Role, "ADMIN")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(
            AuthorizationPolicy policy,
            AuthenticateResult authenticationResult,
            HttpContext context,
            object? resource)
        {
            return Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }

    public class EmpresaWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IEmpresaUseCase> EmpresaUseCaseMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Autenticação fake
                services.AddAuthentication("TestAuth")
                    .AddScheme<AuthenticationSchemeOptions, EmpresaTestAuthHandler>(
                        "TestAuth", options => { });

                // Substitui a policy real pela fake
                services.AddSingleton<IPolicyEvaluator, EmpresaFakePolicyEvaluator>();

                // Remove implementação real e injeta o mock
                services.RemoveAll(typeof(IEmpresaUseCase));
                services.AddSingleton(EmpresaUseCaseMock.Object);
            });
        }
    }

    [Trait("Controller", "Empresa")]
    public class EmpresaControllerTest : IClassFixture<EmpresaWebApplicationFactory>
    {
        private readonly EmpresaWebApplicationFactory _factory;

        public EmpresaControllerTest(EmpresaWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "GET /api/v1/empresa - Retorna 200 quando há empresas")]
        public async Task Get_DeveRetornarOk()
        {
            // Arrange
            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: new[]
                {
                    new EmpresaEntity { Id = 1, Nome = "Empresa 1" },
                    new EmpresaEntity { Id = 2, Nome = "Empresa 2" }
                }
            );

            var retorno = OperationResult<PageData<EmpresaEntity>>
                .Success(page, (int)HttpStatusCode.OK);

            _factory.EmpresaUseCaseMock
                .Setup(x => x.ObterTodasEmpresasAsync(1, 10))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/empresa?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/empresa/{id} - Retorna 200 quando empresa existe")]
        public async Task GetById_DeveRetornarOk()
        {
            // Arrange
            var empresa = new EmpresaEntity { Id = 1, Nome = "Empresa 1" };

            var retorno = OperationResult<EmpresaEntity?>
                .Success(empresa, (int)HttpStatusCode.OK);

            _factory.EmpresaUseCaseMock
                .Setup(x => x.ObterUmaEmpresaAsync(1))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/empresa/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/empresa/{id} - Retorna 404 quando empresa não existe")]
        public async Task GetById_DeveRetornarNotFound()
        {
            // Arrange
            var retorno = OperationResult<EmpresaEntity?>
                .Failure("Empresa não encontrada", (int)HttpStatusCode.NotFound);

            _factory.EmpresaUseCaseMock
                .Setup(x => x.ObterUmaEmpresaAsync(999))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/empresa/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /api/v1/empresa/{id} - Retorna 200 quando deleta com sucesso")]
        public async Task Delete_DeveRetornarOk()
        {
            // Arrange
            var empresa = new EmpresaEntity { Id = 1, Nome = "Empresa 1" };

            var retorno = OperationResult<EmpresaEntity?>
                .Success(empresa, (int)HttpStatusCode.OK);

            _factory.EmpresaUseCaseMock
                .Setup(x => x.DeletarEmpresaAsync(1))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("api/v1/empresa/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /api/v1/empresa/{id} - Retorna 404 quando empresa não existe")]
        public async Task Delete_DeveRetornarNotFound()
        {
            // Arrange
            var retorno = OperationResult<EmpresaEntity?>
                .Failure("Empresa não encontrada", (int)HttpStatusCode.NotFound);

            _factory.EmpresaUseCaseMock
                .Setup(x => x.DeletarEmpresaAsync(999))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("api/v1/empresa/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
