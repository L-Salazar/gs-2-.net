using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Models;
using RemoteReady.UseCases;
using Xunit;

namespace RemoteReady.Tests.UseCases
{
    public class BlogPostUseCaseTest
    {
        private readonly Mock<IBlogPostRepository> _repositoryMock;
        private readonly BlogPostUseCase _useCase;

        public BlogPostUseCaseTest()
        {
            _repositoryMock = new Mock<IBlogPostRepository>();
            _useCase = new BlogPostUseCase(_repositoryMock.Object);
        }

        private static BlogPostEntity BuildPost(
            int id = 1,
            string titulo = "Post Teste",
            string? descricao = "Descrição teste",
            string? imageUrl = "https://image.com/img.png",
            string? tag = "carreira")
        {
            return new BlogPostEntity
            {
                Id = id,
                Titulo = titulo,
                Descricao = descricao,
                ImageUrl = imageUrl,
                Tag = tag,
                DataCriacao = DateTime.UtcNow
            };
        }

        [Fact(DisplayName = "ObterTodosPostsAsync - Retorna posts quando existir dados")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterTodos_DeveRetornarPosts()
        {
            // Arrange
            var posts = new[]
            {
                BuildPost(1, "Post 1"),
                BuildPost(2, "Post 2")
            };

            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: posts
            );

            _repositoryMock
                .Setup(r => r.ObterTodosAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var result = await _useCase.ObterTodosPostsAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value!.TotalRegistros);
        }

        [Fact(DisplayName = "ObterTodosPostsAsync - Retorna NoContent quando não há posts")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterTodos_DeveRetornarNoContent_QuandoNaoHaPosts()
        {
            // Arrange
            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 0,
                totalRegistros: 0,
                itens: Enumerable.Empty<BlogPostEntity>()
            );

            _repositoryMock
                .Setup(r => r.ObterTodosAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var result = await _useCase.ObterTodosPostsAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact(DisplayName = "ObterTodosPostsAsync - Retorna InternalServerError em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterTodos_DeveRetornarErroInterno_EmExcecao()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.ObterTodosAsync(1, 10))
                .ThrowsAsync(new Exception("Erro qualquer"));

            // Act
            var result = await _useCase.ObterTodosPostsAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)result.StatusCode);
        }

        [Fact(DisplayName = "ObterUmPostAsync - Retorna post quando existir")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterUm_DeveRetornarPost()
        {
            // Arrange
            var post = BuildPost(1, "Post 1");

            _repositoryMock
                .Setup(r => r.ObterUmAsync(1))
                .ReturnsAsync(post);

            // Act
            var result = await _useCase.ObterUmPostAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("Post 1", result.Value!.Titulo);
        }

        [Fact(DisplayName = "ObterUmPostAsync - Retorna NotFound quando não existir")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterUm_DeveRetornarNotFound_QuandoNaoExistir()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.ObterUmAsync(999))
                .ReturnsAsync((BlogPostEntity?)null);

            // Act
            var result = await _useCase.ObterUmPostAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact(DisplayName = "ObterUmPostAsync - Retorna InternalServerError em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterUm_DeveRetornarErroInterno_EmExcecao()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.ObterUmAsync(1))
                .ThrowsAsync(new Exception("Erro qualquer"));

            // Act
            var result = await _useCase.ObterUmPostAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)result.StatusCode);
        }

        [Fact(DisplayName = "AdicionarPostAsync - Adiciona post com sucesso")]
        [Trait("UseCase", "BlogPost")]
        public async Task Adicionar_DeveAdicionarPost()
        {
            // Arrange
            var dto = new BlogPostDto(
                Titulo: "Novo Post",
                Descricao: "Desc",
                ImageUrl: "https://image.com/img.png",
                Tag: "carreira"
            );

            var post = BuildPost(1, "Novo Post");

            _repositoryMock
                .Setup(r => r.AdicionarAsync(It.IsAny<BlogPostEntity>()))
                .ReturnsAsync(post);

            // Act
            var result = await _useCase.AdicionarPostAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("Novo Post", result.Value!.Titulo);
        }

        [Fact(DisplayName = "AdicionarPostAsync - Retorna BadRequest em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task Adicionar_DeveRetornarBadRequest_EmExcecao()
        {
            // Arrange
            var dto = new BlogPostDto("Novo Post", "Desc", null, null);

            _repositoryMock
                .Setup(r => r.AdicionarAsync(It.IsAny<BlogPostEntity>()))
                .ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _useCase.AdicionarPostAsync(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
        }

        [Fact(DisplayName = "EditarPostAsync - Edita post com sucesso")]
        [Trait("UseCase", "BlogPost")]
        public async Task Editar_DeveEditarPost()
        {
            // Arrange
            var dto = new BlogPostDto(
                Titulo: "Post Editado",
                Descricao: "Desc nova",
                ImageUrl: null,
                Tag: "nova"
            );

            var postEditado = BuildPost(1, "Post Editado", "Desc nova", null, "nova");

            _repositoryMock
                .Setup(r => r.EditarAsync(1, It.IsAny<BlogPostEntity>()))
                .ReturnsAsync(postEditado);

            // Act
            var result = await _useCase.EditarPostAsync(1, dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("Post Editado", result.Value!.Titulo);
        }

        [Fact(DisplayName = "EditarPostAsync - Retorna NotFound quando post não existir")]
        [Trait("UseCase", "BlogPost")]
        public async Task Editar_DeveRetornarNotFound_QuandoNaoExistir()
        {
            // Arrange
            var dto = new BlogPostDto("Post", "Desc", null, null);

            _repositoryMock
                .Setup(r => r.EditarAsync(999, It.IsAny<BlogPostEntity>()))
                .ReturnsAsync((BlogPostEntity?)null);

            // Act
            var result = await _useCase.EditarPostAsync(999, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact(DisplayName = "EditarPostAsync - Retorna BadRequest em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task Editar_DeveRetornarBadRequest_EmExcecao()
        {
            // Arrange
            var dto = new BlogPostDto("Post", "Desc", null, null);

            _repositoryMock
                .Setup(r => r.EditarAsync(1, It.IsAny<BlogPostEntity>()))
                .ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _useCase.EditarPostAsync(1, dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
        }

        [Fact(DisplayName = "DeletarPostAsync - Deleta post com sucesso")]
        [Trait("UseCase", "BlogPost")]
        public async Task Deletar_DeveDeletarPost()
        {
            // Arrange
            var post = BuildPost(1, "Post 1");

            _repositoryMock
                .Setup(r => r.DeletarAsync(1))
                .ReturnsAsync(post);

            // Act
            var result = await _useCase.DeletarPostAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal("Post 1", result.Value!.Titulo);
        }

        [Fact(DisplayName = "DeletarPostAsync - Retorna NotFound quando não existir")]
        [Trait("UseCase", "BlogPost")]
        public async Task Deletar_DeveRetornarNotFound_QuandoNaoExistir()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.DeletarAsync(999))
                .ReturnsAsync((BlogPostEntity?)null);

            // Act
            var result = await _useCase.DeletarPostAsync(999);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact(DisplayName = "DeletarPostAsync - Retorna BadRequest em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task Deletar_DeveRetornarBadRequest_EmExcecao()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.DeletarAsync(1))
                .ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _useCase.DeletarPostAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
        }

        [Fact(DisplayName = "ObterPostsPorTagAsync - Retorna posts quando existir")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterPorTag_DeveRetornarPosts()
        {
            // Arrange
            var posts = new[]
            {
                BuildPost(1, "Post 1", tag: "carreira"),
                BuildPost(2, "Post 2", tag: "carreira")
            };

            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: posts
            );

            _repositoryMock
                .Setup(r => r.ObterPorTagAsync("carreira", 1, 10))
                .ReturnsAsync(page);

            // Act
            var result = await _useCase.ObterPostsPorTagAsync("carreira", 1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value!.TotalRegistros);
        }

        [Fact(DisplayName = "ObterPostsPorTagAsync - Retorna NoContent quando não houver posts")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterPorTag_DeveRetornarNoContent_QuandoNaoHaPosts()
        {
            // Arrange
            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 0,
                totalRegistros: 0,
                itens: Enumerable.Empty<BlogPostEntity>()
            );

            _repositoryMock
                .Setup(r => r.ObterPorTagAsync("carreira", 1, 10))
                .ReturnsAsync(page);

            // Act
            var result = await _useCase.ObterPostsPorTagAsync("carreira", 1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact(DisplayName = "ObterPostsPorTagAsync - Retorna InternalServerError em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterPorTag_DeveRetornarErroInterno_EmExcecao()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.ObterPorTagAsync("carreira", 1, 10))
                .ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _useCase.ObterPostsPorTagAsync("carreira", 1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)result.StatusCode);
        }

        [Fact(DisplayName = "ObterPostsRecentesAsync - Retorna posts recentes quando existir dados")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterRecentes_DeveRetornarPosts()
        {
            // Arrange
            var posts = new[]
            {
                BuildPost(1, "Post 1"),
                BuildPost(2, "Post 2")
            };

            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: posts
            );

            _repositoryMock
                .Setup(r => r.ObterRecentesAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var result = await _useCase.ObterPostsRecentesAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value!.TotalRegistros);
        }

        [Fact(DisplayName = "ObterPostsRecentesAsync - Retorna NoContent quando não houver posts")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterRecentes_DeveRetornarNoContent_QuandoNaoHaPosts()
        {
            // Arrange
            var page = new PageData<BlogPostEntity>(
                paginaAtual: 1,
                totalPaginas: 0,
                totalRegistros: 0,
                itens: Enumerable.Empty<BlogPostEntity>()
            );

            _repositoryMock
                .Setup(r => r.ObterRecentesAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var result = await _useCase.ObterPostsRecentesAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)result.StatusCode);
            Assert.Null(result.Value);
        }

        [Fact(DisplayName = "ObterPostsRecentesAsync - Retorna InternalServerError em exceção")]
        [Trait("UseCase", "BlogPost")]
        public async Task ObterRecentes_DeveRetornarErroInterno_EmExcecao()
        {
            // Arrange
            _repositoryMock
                .Setup(r => r.ObterRecentesAsync(1, 10))
                .ThrowsAsync(new Exception("Erro"));

            // Act
            var result = await _useCase.ObterPostsRecentesAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, (HttpStatusCode)result.StatusCode);
        }
    }
}
