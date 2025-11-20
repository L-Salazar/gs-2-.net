using Moq;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Models;
using RemoteReady.UseCases;
using System.Net;

namespace RemoteReady.Tests.UseCases
{
    public class UsuarioUseCaseTest
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly UsuarioUseCase _useCase;

        public UsuarioUseCaseTest()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _useCase = new UsuarioUseCase(_repositoryMock.Object);
        }

        private static UsuarioEntity BuildUsuario(
            int id = 1,
            string? nome = null,
            string? email = null,
            string? tipoUsuario = null)
        {
            return new UsuarioEntity
            {
                Id = id,
                Nome = nome ?? "João Silva",
                Email = email ?? "joao@email.com",
                Senha = "senha123",
                TipoUsuario = tipoUsuario ?? "USER",
                DataCriacao = DateTime.Now
            };
        }

        [Fact(DisplayName = "ObterTodosUsuariosAsync - Retorna usuários com sucesso")]
        [Trait("UseCase", "Usuario")]
        public async Task ObterTodos_DeveRetornarUsuarios()
        {
            // Arrange
            var usuarios = new List<UsuarioEntity>
            {
                BuildUsuario(1, "João Silva", "joao@email.com"),
                BuildUsuario(2, "Maria Santos", "maria@email.com")
            };

            var pageData = new PageData<UsuarioEntity>(1, 1, 2, usuarios);

            _repositoryMock
                .Setup(x => x.ObterTodosAsync(1, 10))
                .ReturnsAsync(pageData);

            // Act
            var resultado = await _useCase.ObterTodosUsuariosAsync(1, 10);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal(2, resultado.Value.TotalRegistros);
        }

        [Fact(DisplayName = "ObterTodosUsuariosAsync - Retorna NoContent quando não há usuários")]
        [Trait("UseCase", "Usuario")]
        public async Task ObterTodos_DeveRetornarNoContent_QuandoNaoHaUsuarios()
        {
            // Arrange
            var pageData = new PageData<UsuarioEntity>(1, 0, 0, new List<UsuarioEntity>());

            _repositoryMock
                .Setup(x => x.ObterTodosAsync(1, 10))
                .ReturnsAsync(pageData);

            // Act
            var resultado = await _useCase.ObterTodosUsuariosAsync(1, 10);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)resultado.StatusCode);
        }

        [Fact(DisplayName = "ObterUmUsuarioAsync - Retorna usuário por ID")]
        [Trait("UseCase", "Usuario")]
        public async Task ObterUm_DeveRetornarUsuario()
        {
            // Arrange
            var usuario = BuildUsuario(1, "João Silva", "joao@email.com");

            _repositoryMock
                .Setup(x => x.ObterUmAsync(1))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _useCase.ObterUmUsuarioAsync(1);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("João Silva", resultado.Value.Nome);
        }

        [Fact(DisplayName = "ObterUmUsuarioAsync - Retorna NotFound quando não encontra")]
        [Trait("UseCase", "Usuario")]
        public async Task ObterUm_DeveRetornarNotFound_QuandoNaoEncontra()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.ObterUmAsync(999))
                .ReturnsAsync((UsuarioEntity?)null);

            // Act
            var resultado = await _useCase.ObterUmUsuarioAsync(999);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)resultado.StatusCode);
        }

        [Fact(DisplayName = "AdicionarUsuarioAsync - Adiciona usuário com sucesso")]
        [Trait("UseCase", "Usuario")]
        public async Task Adicionar_DeveAdicionarUsuario()
        {
            // Arrange
            var dto = new UsuarioDto(
                "João Silva",
                "joao@email.com",
                "senha123",
                "USER"
            );

            var usuario = BuildUsuario(1, "João Silva", "joao@email.com");


            _repositoryMock
                .Setup(x => x.AdicionarAsync(It.IsAny<UsuarioEntity>()))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _useCase.AdicionarUsuarioAsync(dto);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("João Silva", resultado.Value.Nome);
        }

        [Fact(DisplayName = "EditarUsuarioAsync - Edita usuário com sucesso")]
        [Trait("UseCase", "Usuario")]
        public async Task Editar_DeveEditarUsuario()
        {
            // Arrange
            var dto = new UsuarioDto(
                "João Silva Editado",
                "joao@email.com",
                "novasenha",
                "ADMIN"
            );

            var usuarioEditado = BuildUsuario(1, "João Silva Editado", "joao@email.com", "ADMIN");

            _repositoryMock
                .Setup(x => x.EditarAsync(1, It.IsAny<UsuarioEntity>()))
                .ReturnsAsync(usuarioEditado);

            // Act
            var resultado = await _useCase.EditarUsuarioAsync(1, dto);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("João Silva Editado", resultado.Value.Nome);
            Assert.Equal("ADMIN", resultado.Value.TipoUsuario);
        }

        [Fact(DisplayName = "EditarUsuarioAsync - Retorna NotFound quando usuário não existe")]
        [Trait("UseCase", "Usuario")]
        public async Task Editar_DeveRetornarNotFound_QuandoUsuarioNaoExiste()
        {
            // Arrange
            var dto = new UsuarioDto(
                "João Silva",
                "joao@email.com",
                "senha123",
                "USER"
            );

            _repositoryMock
                .Setup(x => x.EditarAsync(999, It.IsAny<UsuarioEntity>()))
                .ReturnsAsync((UsuarioEntity?)null);

            // Act
            var resultado = await _useCase.EditarUsuarioAsync(999, dto);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)resultado.StatusCode);
        }

        [Fact(DisplayName = "DeletarUsuarioAsync - Deleta usuário com sucesso")]
        [Trait("UseCase", "Usuario")]
        public async Task Deletar_DeveDeletarUsuario()
        {
            // Arrange
            var usuario = BuildUsuario(1, "João Silva", "joao@email.com");

            _repositoryMock
                .Setup(x => x.DeletarAsync(1))
                .ReturnsAsync(usuario);

            // Act
            var resultado = await _useCase.DeletarUsuarioAsync(1);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
        }

        [Fact(DisplayName = "DeletarUsuarioAsync - Retorna NotFound quando usuário não existe")]
        [Trait("UseCase", "Usuario")]
        public async Task Deletar_DeveRetornarNotFound_QuandoUsuarioNaoExiste()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.DeletarAsync(999))
                .ReturnsAsync((UsuarioEntity?)null);

            // Act
            var resultado = await _useCase.DeletarUsuarioAsync(999);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)resultado.StatusCode);
        }

    }
}