using Microsoft.EntityFrameworkCore;
using RemoteReady.Data.AppData;
using RemoteReady.Data.Repositories;
using RemoteReady.Models;

namespace RemoteReady.Tests.Repositories
{
    public class UsuarioRepositoryTest
    {
        private ApplicationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        private static UsuarioEntity BuildUsuario(
            string? nome = null,
            string? email = null,
            string? senha = null,
            string? tipoUsuario = null)
        {
            return new UsuarioEntity
            {
                Nome = nome ?? "João Silva",
                Email = email ?? "joao@email.com",
                Senha = senha ?? "senha123",
                TipoUsuario = tipoUsuario ?? "USER"
            };
        }

        [Fact(DisplayName = "ObterTodosAsync - Retorna todos os usuários")]
        [Trait("Repository", "Usuario")]
        public async Task ObterTodos_DeveRetornarValor()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario1 = BuildUsuario("João Silva", "joao@email.com");
            var usuario2 = BuildUsuario("Maria Santos", "maria@email.com");
            var usuario3 = BuildUsuario("Pedro Costa", "pedro@email.com");

            // Preparando o banco em memória
            context.Usuarios.AddRange(usuario1, usuario2, usuario3);
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.ObterTodosAsync(1, 2);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(3, retorno.TotalRegistros);
            Assert.Equal(1, retorno.PaginaAtual);
            Assert.Equal(2, retorno.Itens.Count());
            Assert.NotNull(retorno.Itens);
        }

        [Fact(DisplayName = "ObterUmAsync - Retorna um usuário por ID")]
        [Trait("Repository", "Usuario")]
        public async Task ObterUm_DeveRetornarUsuario()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario = BuildUsuario("João Silva", "joao@email.com");
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.ObterUmAsync(usuario.Id);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(usuario.Id, retorno.Id);
            Assert.Equal("João Silva", retorno.Nome);
            Assert.Equal("joao@email.com", retorno.Email);
        }

        [Fact(DisplayName = "ObterUmAsync - Retorna null quando não encontra")]
        [Trait("Repository", "Usuario")]
        public async Task ObterUm_DeveRetornarNull_QuandoNaoEncontrar()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            // Act
            var retorno = await repository.ObterUmAsync(999);

            // Assert
            Assert.Null(retorno);
        }

        [Fact(DisplayName = "AdicionarAsync - Adiciona um usuário")]
        [Trait("Repository", "Usuario")]
        public async Task Adicionar_DeveAdicionarUsuario()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario = BuildUsuario("Novo Usuario", "novo@email.com");

            // Act
            var retorno = await repository.AdicionarAsync(usuario);

            // Assert
            Assert.NotNull(retorno);
            Assert.True(retorno.Id > 0);
            Assert.Equal("Novo Usuario", retorno.Nome);
            Assert.Equal("novo@email.com", retorno.Email);

            var usuarioNoBanco = await context.Usuarios.FindAsync(retorno.Id);
            Assert.NotNull(usuarioNoBanco);
        }

        [Fact(DisplayName = "EditarAsync - Edita um usuário existente")]
        [Trait("Repository", "Usuario")]
        public async Task Editar_DeveEditarUsuario()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario = BuildUsuario("Usuario Original", "original@email.com");
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            var usuarioEditado = BuildUsuario("Usuario Editado", "editado@email.com", "novasenha", "ADMIN");

            // Act
            var retorno = await repository.EditarAsync(usuario.Id, usuarioEditado);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal("Usuario Editado", retorno.Nome);
            Assert.Equal("editado@email.com", retorno.Email);
            Assert.Equal("novasenha", retorno.Senha);
            Assert.Equal("ADMIN", retorno.TipoUsuario);
        }

        [Fact(DisplayName = "EditarAsync - Retorna null quando usuário não existe")]
        [Trait("Repository", "Usuario")]
        public async Task Editar_DeveRetornarNull_QuandoUsuarioNaoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuarioEditado = BuildUsuario("Usuario Editado", "editado@email.com");

            // Act
            var retorno = await repository.EditarAsync(999, usuarioEditado);

            // Assert
            Assert.Null(retorno);
        }

        [Fact(DisplayName = "DeletarAsync - Deleta um usuário")]
        [Trait("Repository", "Usuario")]
        public async Task Deletar_DeveDeletarUsuario()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario = BuildUsuario("Usuario Teste", "teste@email.com");
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.DeletarAsync(usuario.Id);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(usuario.Id, retorno.Id);

            var usuarioNoBanco = await context.Usuarios.FindAsync(usuario.Id);
            Assert.Null(usuarioNoBanco);
        }

        [Fact(DisplayName = "DeletarAsync - Retorna null quando usuário não existe")]
        [Trait("Repository", "Usuario")]
        public async Task Deletar_DeveRetornarNull_QuandoUsuarioNaoExiste()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            // Act
            var retorno = await repository.DeletarAsync(999);

            // Assert
            Assert.Null(retorno);
        }

        [Fact(DisplayName = "ObterTotalAsync - Retorna total de usuários")]
        [Trait("Repository", "Usuario")]
        public async Task ObterTotal_DeveRetornarTotalDeUsuarios()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario1 = BuildUsuario("João Silva", "joao@email.com");
            var usuario2 = BuildUsuario("Maria Santos", "maria@email.com");
            var usuario3 = BuildUsuario("Pedro Costa", "pedro@email.com");

            context.Usuarios.AddRange(usuario1, usuario2, usuario3);
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.ObterTotalAsync();

            // Assert
            Assert.Equal(3, retorno);
        }

        [Fact(DisplayName = "ObterPorEmailAsync - Retorna usuário por email")]
        [Trait("Repository", "Usuario")]
        public async Task ObterPorEmail_DeveRetornarUsuario()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            var usuario = BuildUsuario("João Silva", "joao@email.com");
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.ObterPorEmailAsync("joao@email.com");

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal("joao@email.com", retorno.Email);
            Assert.Equal("João Silva", retorno.Nome);
        }

        [Fact(DisplayName = "ObterPorEmailAsync - Retorna null quando não encontra")]
        [Trait("Repository", "Usuario")]
        public async Task ObterPorEmail_DeveRetornarNull_QuandoNaoEncontrar()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new UsuarioRepository(context);

            // Act
            var retorno = await repository.ObterPorEmailAsync("naoexiste@email.com");

            // Assert
            Assert.Null(retorno);
        }
    }
}