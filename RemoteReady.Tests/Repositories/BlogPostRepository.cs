using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RemoteReady.Data.AppData;
using RemoteReady.Data.Repositories;
using RemoteReady.Models;
using Xunit;

namespace RemoteReady.Tests.Repositories
{
    public class BlogPostRepositoryTest
    {
        private ApplicationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        private static BlogPostEntity BuildPost(
            int id = 0,
            string titulo = "Post Teste",
            string? descricao = "Descrição teste",
            string? imageUrl = "https://image.com/img.png",
            string? tag = "carreira",
            DateTime? dataCriacao = null)
        {
            return new BlogPostEntity
            {
                Id = id,
                Titulo = titulo,
                Descricao = descricao,
                ImageUrl = imageUrl,
                Tag = tag,
                DataCriacao = dataCriacao ?? DateTime.UtcNow
            };
        }

        [Fact(DisplayName = "ObterTodosAsync - Retorna posts paginados")]
        [Trait("Repository", "BlogPost")]
        public async Task ObterTodosAsync_DeveRetornarPostsPaginados()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            context.BlogPosts.AddRange(
                BuildPost(titulo: "Post 1", dataCriacao: DateTime.UtcNow.AddMinutes(-10)),
                BuildPost(titulo: "Post 2", dataCriacao: DateTime.UtcNow.AddMinutes(-5)),
                BuildPost(titulo: "Post 3", dataCriacao: DateTime.UtcNow)
            );
            await context.SaveChangesAsync();

            // Act
            var page = await repository.ObterTodosAsync(1, 2);

            // Assert
            Assert.NotNull(page);
            Assert.Equal(3, page.TotalRegistros);
            Assert.Equal(2, page.Itens.Count());
            // Deve vir ordenado por DataCriacao desc (mais recente primeiro)
            Assert.Equal("Post 3", page.Itens.First().Titulo);
        }

        [Fact(DisplayName = "ObterUmAsync - Retorna post existente")]
        [Trait("Repository", "BlogPost")]
        public async Task ObterUmAsync_DeveRetornarPostExistente()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            var post = BuildPost(titulo: "Post Único");
            context.BlogPosts.Add(post);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.ObterUmAsync(post.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Post Único", result!.Titulo);
        }

        [Fact(DisplayName = "ObterUmAsync - Retorna null quando não encontrar")]
        [Trait("Repository", "BlogPost")]
        public async Task ObterUmAsync_DeveRetornarNull_QuandoNaoEncontrar()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            // Act
            var result = await repository.ObterUmAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "AdicionarAsync - Adiciona post com sucesso")]
        [Trait("Repository", "BlogPost")]
        public async Task AdicionarAsync_DeveAdicionarPost()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            var post = BuildPost(titulo: "Novo Post");

            // Act
            var result = await repository.AdicionarAsync(post);

            // Assert
            Assert.NotNull(result);
            Assert.True(result!.Id > 0);
            Assert.Equal("Novo Post", result.Titulo);
            Assert.True(await context.BlogPosts.AnyAsync(p => p.Id == result.Id));
        }

        [Fact(DisplayName = "EditarAsync - Edita post existente")]
        [Trait("Repository", "BlogPost")]
        public async Task EditarAsync_DeveEditarPostExistente()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            var post = BuildPost(titulo: "Título antigo", descricao: "Desc antiga");
            context.BlogPosts.Add(post);
            await context.SaveChangesAsync();

            var novo = BuildPost(titulo: "Título novo", descricao: "Desc nova", imageUrl: "https://image.com/new.png", tag: "tagnova");

            // Act
            var result = await repository.EditarAsync(post.Id, novo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Título novo", result!.Titulo);
            Assert.Equal("Desc nova", result.Descricao);
            Assert.Equal("https://image.com/new.png", result.ImageUrl);
            Assert.Equal("tagnova", result.Tag);
        }

        [Fact(DisplayName = "EditarAsync - Retorna null quando post não existe")]
        [Trait("Repository", "BlogPost")]
        public async Task EditarAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            var novo = BuildPost(titulo: "Fantasma");

            // Act
            var result = await repository.EditarAsync(999, novo);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "DeletarAsync - Remove post existente")]
        [Trait("Repository", "BlogPost")]
        public async Task DeletarAsync_DeveRemoverPost()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            var post = BuildPost(titulo: "Para deletar");
            context.BlogPosts.Add(post);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.DeletarAsync(post.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(post.Id, result!.Id);
            Assert.False(await context.BlogPosts.AnyAsync(p => p.Id == post.Id));
        }

        [Fact(DisplayName = "DeletarAsync - Retorna null quando post não existe")]
        [Trait("Repository", "BlogPost")]
        public async Task DeletarAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            // Act
            var result = await repository.DeletarAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "ObterTotalAsync - Retorna total de posts")]
        [Trait("Repository", "BlogPost")]
        public async Task ObterTotalAsync_DeveRetornarTotal()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            context.BlogPosts.AddRange(
                BuildPost(titulo: "Post 1"),
                BuildPost(titulo: "Post 2"),
                BuildPost(titulo: "Post 3")
            );
            await context.SaveChangesAsync();

            // Act
            var total = await repository.ObterTotalAsync();

            // Assert
            Assert.Equal(3, total);
        }

        [Fact(DisplayName = "ObterPorTagAsync - Retorna posts filtrados por tag")]
        [Trait("Repository", "BlogPost")]
        public async Task ObterPorTagAsync_DeveRetornarPostsPorTag()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            context.BlogPosts.AddRange(
                BuildPost(titulo: "Post carreira 1", tag: "carreira"),
                BuildPost(titulo: "Post carreira 2", tag: "CarReIrA"),
                BuildPost(titulo: "Post outro", tag: "finanças"),
                BuildPost(titulo: "Post sem tag", tag: null)
            );
            await context.SaveChangesAsync();

            // Act
            var page = await repository.ObterPorTagAsync("carreira", 1, 10);

            // Assert
            Assert.NotNull(page);
            Assert.Equal(2, page.Itens.Count());
            Assert.All(page.Itens, p => Assert.Contains("carreira", p.Tag!.ToLower()));
        }

        [Fact(DisplayName = "ObterRecentesAsync - Retorna posts recentes paginados")]
        [Trait("Repository", "BlogPost")]
        public async Task ObterRecentesAsync_DeveRetornarPostsRecentes()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new BlogPostRepository(context);

            context.BlogPosts.AddRange(
                BuildPost(titulo: "Antigo", dataCriacao: DateTime.UtcNow.AddDays(-2)),
                BuildPost(titulo: "Intermediário", dataCriacao: DateTime.UtcNow.AddDays(-1)),
                BuildPost(titulo: "Mais recente", dataCriacao: DateTime.UtcNow)
            );
            await context.SaveChangesAsync();

            // Act
            var page = await repository.ObterRecentesAsync(1, 2);

            // Assert
            Assert.NotNull(page);
            Assert.Equal(3, page.TotalRegistros);
            Assert.Equal(2, page.Itens.Count());
            Assert.Equal("Mais recente", page.Itens.First().Titulo);
        }
    }
}
