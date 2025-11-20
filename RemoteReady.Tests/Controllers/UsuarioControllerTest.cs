using Microsoft.AspNetCore.Authentication;
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
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace RemoteReady.Tests.Controllers
{
    public class UsuarioTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Scheme = "TestAuth";

        public UsuarioTestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder) : base(options, logger, encoder)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, "tester"),
                new Claim(ClaimTypes.Role, "ADMIN"),
            };

            var identity = new ClaimsIdentity(claims, Scheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public class UsuarioWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IUsuarioUseCase> UsuarioUseCaseMock { get; } = new();

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove o IUsuarioUseCase real e adiciona o mock
                services.RemoveAll(typeof(IUsuarioUseCase));
                services.AddSingleton(UsuarioUseCaseMock.Object);

                // Autenticação fake
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = UsuarioTestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = UsuarioTestAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, UsuarioTestAuthHandler>(UsuarioTestAuthHandler.Scheme, _ => { });
            });
        }
    }

    public class UsuarioControllerTest : IClassFixture<UsuarioWebApplicationFactory>
    {
        private readonly UsuarioWebApplicationFactory _factory;

        public UsuarioControllerTest(UsuarioWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "ObterTodos - Retorna todos os usuários")]
        [Trait("Controller", "Usuario")]
        public async Task ObterTodos_RetornarDados()
        {
            // Arrange
            var retornoUsuarios = new PageData<UsuarioEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: new List<UsuarioEntity>
                {
                    new UsuarioEntity
                    {
                        Id = 1,
                        Nome = "João Silva",
                        Email = "joao@email.com",
                        Senha = "senha123",
                        TipoUsuario = "USER",
                        DataCriacao = DateTime.Now
                    },
                    new UsuarioEntity
                    {
                        Id = 2,
                        Nome = "Maria Santos",
                        Email = "maria@email.com",
                        Senha = "senha456",
                        TipoUsuario = "ADMIN",
                        DataCriacao = DateTime.Now
                    }
                }
            );

            var retorno = OperationResult<PageData<UsuarioEntity>>.Success(retornoUsuarios, 200);

            _factory.UsuarioUseCaseMock
                .Setup(x => x.ObterTodosUsuariosAsync(1, 10))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/usuario?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "ObterTodos - Retorna NoContent quando não há usuários")]
        [Trait("Controller", "Usuario")]
        public async Task ObterTodos_RetornarNoContent()
        {
            // Arrange
            var retorno = OperationResult<PageData<UsuarioEntity>>.Failure("Não há usuários cadastrados", (int)HttpStatusCode.NoContent);

            _factory.UsuarioUseCaseMock
                .Setup(x => x.ObterTodosUsuariosAsync(1, 10))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/usuario?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "ObterPorId - Retorna um usuário")]
        [Trait("Controller", "Usuario")]
        public async Task ObterPorId_RetornarUsuario()
        {
            // Arrange
            var usuario = new UsuarioEntity
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao@email.com",
                Senha = "senha123",
                TipoUsuario = "USER",
                DataCriacao = DateTime.Now
            };

            var retorno = OperationResult<UsuarioEntity?>.Success(usuario, 200);

            _factory.UsuarioUseCaseMock
                .Setup(x => x.ObterUmUsuarioAsync(1))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/usuario/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "ObterPorId - Retorna NotFound quando usuário não existe")]
        [Trait("Controller", "Usuario")]
        public async Task ObterPorId_RetornarNotFound()
        {
            // Arrange
            var retorno = OperationResult<UsuarioEntity?>.Failure("Usuário não encontrado", (int)HttpStatusCode.NotFound);

            _factory.UsuarioUseCaseMock
                .Setup(x => x.ObterUmUsuarioAsync(999))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/usuario/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "Delete - Deleta um usuário")]
        [Trait("Controller", "Usuario")]
        public async Task Delete_DeletarUsuario()
        {
            // Arrange
            var usuarioDeletado = new UsuarioEntity
            {
                Id = 1,
                Nome = "Usuario Teste",
                Email = "teste@email.com",
                Senha = "senha123",
                TipoUsuario = "USER",
                DataCriacao = DateTime.Now
            };

            var retorno = OperationResult<UsuarioEntity?>.Success(usuarioDeletado, 200);

            _factory.UsuarioUseCaseMock
                .Setup(x => x.DeletarUsuarioAsync(1))
                .ReturnsAsync(retorno);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("api/v1/usuario/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
     }
    
}