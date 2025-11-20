using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
using RemoteReady;
using RemoteReady.Dtos;
using RemoteReady.Interfaces;
using RemoteReady.Models;
using Xunit;

namespace RemoteReady.Tests.Controllers
{
    // Auth handler fake para testes
    public class BlogPostTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BlogPostTestAuthHandler(
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

    // Policy evaluator fake para ignorar a autorização real
    public class BlogPostFakePolicyEvaluator : IPolicyEvaluator
    {
        public Task<AuthenticateResult> AuthenticateAsync(
            AuthorizationPolicy policy,
            HttpContext context)
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

    public class BlogPostWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IBlogPostUseCase> BlogPostUseCaseMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Autenticação fake
                services.AddAuthentication("TestAuth")
                    .AddScheme<AuthenticationSchemeOptions, BlogPostTestAuthHandler>(
                        "TestAuth", _ => { });

                // Substitui o avaliador de política pela versão fake
                services.AddSingleton<IPolicyEvaluator, BlogPostFakePolicyEvaluator>();

                // Substitui o IBlogPostUseCase real pelo mock
                services.RemoveAll(typeof(IBlogPostUseCase));
                services.AddSingleton(BlogPostUseCaseMock.Object);
            });
        }
    }

    [Trait("Controller", "BlogPost")]
    public class BlogPostsControllerTest : IClassFixture<BlogPostWebApplicationFactory>
    {
        private readonly BlogPostWebApplicationFactory _factory;

        public BlogPostsControllerTest(BlogPostWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "GET /api/v1/blogpost - Retorna 200 quando há posts")]
        public async Task Get_DeveRetornarOk()
        {
            // Arrange
            var posts = new[]
            {
                new BlogPostEntity { Id = 1, Titulo = "Post 1" },
                new BlogPostEntity { Id = 2, Titulo = "Post 2" }
            };

            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: posts
            );

            var result = OperationResult<PageData<BlogPostEntity>>.Success(page, (int)HttpStatusCode.OK);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterTodosPostsAsync(1, 10))
                .ReturnsAsync(result);

            using HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost - Retorna 204 quando não há posts")]
        public async Task Get_DeveRetornarNoContent()
        {
            // Arrange
            var result = OperationResult<PageData<BlogPostEntity>>
                .Failure("Não há posts cadastrados", (int)HttpStatusCode.NoContent);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterTodosPostsAsync(1, 10))
                .ReturnsAsync(result);

            using HttpClient client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost/{id} - Retorna 200 quando post existe")]
        public async Task GetById_DeveRetornarOk()
        {
            // Arrange
            var post = new BlogPostEntity { Id = 1, Titulo = "Post 1" };

            var result = OperationResult<BlogPostEntity?>
                .Success(post, (int)HttpStatusCode.OK);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterUmPostAsync(1))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost/{id} - Retorna 404 quando não existe")]
        public async Task GetById_DeveRetornarNotFound()
        {
            // Arrange
            var result = OperationResult<BlogPostEntity?>
                .Failure("Post não encontrado", (int)HttpStatusCode.NotFound);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterUmPostAsync(999))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost/tag/{tag} - Retorna 200 quando há posts")]
        public async Task GetPorTag_DeveRetornarOk()
        {
            // Arrange
            var posts = new[]
            {
                new BlogPostEntity { Id = 1, Titulo = "Post carreira 1", Tag = "carreira" }
            };

            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 1,
                itens: posts
            );

            var result = OperationResult<PageData<BlogPostEntity>>
                .Success(page, (int)HttpStatusCode.OK);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterPostsPorTagAsync("carreira", 1, 10))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost/tag/carreira?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost/tag/{tag} - Retorna 204 quando não há posts")]
        public async Task GetPorTag_DeveRetornarNoContent()
        {
            // Arrange
            var result = OperationResult<PageData<BlogPostEntity>>
                .Failure("Não há posts com a tag 'carreira'", (int)HttpStatusCode.NoContent);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterPostsPorTagAsync("carreira", 1, 10))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost/tag/carreira?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost/recentes - Retorna 200 quando há posts")]
        public async Task GetRecentes_DeveRetornarOk()
        {
            // Arrange
            var posts = new[]
            {
                new BlogPostEntity { Id = 1, Titulo = "Recente 1" }
            };

            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 1,
                itens: posts
            );

            var result = OperationResult<PageData<BlogPostEntity>>
                .Success(page, (int)HttpStatusCode.OK);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterPostsRecentesAsync(1, 10))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost/recentes?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "GET /api/v1/blogpost/recentes - Retorna 204 quando não há posts")]
        public async Task GetRecentes_DeveRetornarNoContent()
        {
            // Arrange
            var result = OperationResult<PageData<BlogPostEntity>>
                .Failure("Não há posts cadastrados", (int)HttpStatusCode.NoContent);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.ObterPostsRecentesAsync(1, 10))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("api/v1/blogpost/recentes?PaginaAtual=1&LimitePagina=10");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "POST /api/v1/blogpost - Retorna 201 quando cria com sucesso")]
        public async Task Post_DeveRetornarCreated()
        {
            // Arrange
            var dto = new BlogPostDto(
                Titulo: "Novo Post",
                Descricao: "Desc",
                ImageUrl: "https://image.com/img.png",
                Tag: "carreira"
            );

            var post = new BlogPostEntity { Id = 1, Titulo = "Novo Post" };

            var result = OperationResult<BlogPostEntity?>
                .Success(post, (int)HttpStatusCode.Created);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.AdicionarPostAsync(dto))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("api/v1/blogpost", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact(DisplayName = "POST /api/v1/blogpost - Retorna 400 quando erro de validação/negócio")]
        public async Task Post_DeveRetornarBadRequest()
        {
            // Arrange
            var dto = new BlogPostDto("Novo Post", "Desc", null, null);

            var result = OperationResult<BlogPostEntity?>
                .Failure("Erro ao salvar", (int)HttpStatusCode.BadRequest);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.AdicionarPostAsync(dto))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.PostAsJsonAsync("api/v1/blogpost", dto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(DisplayName = "PUT /api/v1/blogpost/{id} - Retorna 200 quando atualiza com sucesso")]
        public async Task Put_DeveRetornarOk()
        {
            // Arrange
            var dto = new BlogPostDto("Editado", "Desc nova", null, "tag");
            var postEditado = new BlogPostEntity { Id = 1, Titulo = "Editado" };

            var result = OperationResult<BlogPostEntity?>
                .Success(postEditado, (int)HttpStatusCode.OK);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.EditarPostAsync(1, dto))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsJsonAsync("api/v1/blogpost/1", dto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "PUT /api/v1/blogpost/{id} - Retorna 404 quando post não existe")]
        public async Task Put_DeveRetornarNotFound()
        {
            // Arrange
            var dto = new BlogPostDto("Editado", "Desc", null, null);

            var result = OperationResult<BlogPostEntity?>
                .Failure("Post não encontrado", (int)HttpStatusCode.NotFound);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.EditarPostAsync(999, dto))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.PutAsJsonAsync("api/v1/blogpost/999", dto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /api/v1/blogpost/{id} - Retorna 200 quando exclui com sucesso")]
        public async Task Delete_DeveRetornarOk()
        {
            // Arrange
            var post = new BlogPostEntity { Id = 1, Titulo = "Post 1" };

            var result = OperationResult<BlogPostEntity?>
                .Success(post, (int)HttpStatusCode.OK);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.DeletarPostAsync(1))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("api/v1/blogpost/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact(DisplayName = "DELETE /api/v1/blogpost/{id} - Retorna 404 quando post não existe")]
        public async Task Delete_DeveRetornarNotFound()
        {
            // Arrange
            var result = OperationResult<BlogPostEntity?>
                .Failure("Post não encontrado", (int)HttpStatusCode.NotFound);

            _factory.BlogPostUseCaseMock
                .Setup(x => x.DeletarPostAsync(999))
                .ReturnsAsync(result);

            using var client = _factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("api/v1/blogpost/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
