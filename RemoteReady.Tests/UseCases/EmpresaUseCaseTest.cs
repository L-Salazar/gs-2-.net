using Moq;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Dtos;
using RemoteReady.Models;
using RemoteReady.UseCases;
using System.Net;
using Xunit;

namespace RemoteReady.Tests.UseCases
{
    public class EmpresaUseCaseTest
    {
        private readonly Mock<IEmpresaRepository> _repositoryMock;
        private readonly EmpresaUseCase _useCase;

        public EmpresaUseCaseTest()
        {
            _repositoryMock = new Mock<IEmpresaRepository>();
            _useCase = new EmpresaUseCase(_repositoryMock.Object);
        }

        private static EmpresaEntity BuildEmpresa(
            int id = 1,
            string? nome = null,
            string? area = null,
            string contratandoAgora = "Y")
        {
            return new EmpresaEntity
            {
                Id = id,
                Nome = nome ?? "Empresa Teste",
                Descricao = "Desc teste",
                Area = area ?? "TI",
                ContratandoAgora = contratandoAgora,
                LogoUrl = "https://logo.com/logo.png",
                Website = "https://empresa.com",
                DataCriacao = DateTime.UtcNow
            };
        }

        [Fact(DisplayName = "ObterTodasEmpresasAsync - Retorna empresas quando existem dados")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterTodas_DeveRetornarEmpresas()
        {
            // Arrange
            var empresas = new[]
            {
                BuildEmpresa(1, "Empresa 1"),
                BuildEmpresa(2, "Empresa 2")
            };

            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: empresas
            );

            _repositoryMock
                .Setup(x => x.ObterTodosAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var resultado = await _useCase.ObterTodasEmpresasAsync(1, 10);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal(2, resultado.Value!.TotalRegistros);
        }

        [Fact(DisplayName = "ObterTodasEmpresasAsync - Retorna NoContent quando não há empresas")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterTodas_DeveRetornarNoContent_QuandoNaoHaEmpresas()
        {
            // Arrange
            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 0,
                totalRegistros: 0,
                itens: Enumerable.Empty<EmpresaEntity>()
            );

            _repositoryMock
                .Setup(x => x.ObterTodosAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var resultado = await _useCase.ObterTodasEmpresasAsync(1, 10);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)resultado.StatusCode);
            Assert.Null(resultado.Value);
        }

        [Fact(DisplayName = "ObterUmaEmpresaAsync - Retorna empresa quando existir")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterUma_DeveRetornarEmpresa()
        {
            // Arrange
            var empresa = BuildEmpresa(1, "Empresa 1");

            _repositoryMock
                .Setup(x => x.ObterUmaAsync(1))
                .ReturnsAsync(empresa);

            // Act
            var resultado = await _useCase.ObterUmaEmpresaAsync(1);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("Empresa 1", resultado.Value!.Nome);
        }

        [Fact(DisplayName = "ObterUmaEmpresaAsync - Retorna NotFound quando não existir")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterUma_DeveRetornarNotFound_QuandoNaoExistir()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.ObterUmaAsync(999))
                .ReturnsAsync((EmpresaEntity?)null);

            // Act
            var resultado = await _useCase.ObterUmaEmpresaAsync(999);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)resultado.StatusCode);
            Assert.Null(resultado.Value);
        }

        [Fact(DisplayName = "AdicionarEmpresaAsync - Adiciona empresa com sucesso")]
        [Trait("UseCase", "Empresa")]
        public async Task Adicionar_DeveAdicionarEmpresa()
        {
            // Arrange
            var dto = new EmpresaDto(
                Nome: "Empresa Nova",
                Descricao: "Descrição",
                Area: "TI",
                ContratandoAgora: "Y",
                LogoUrl: "https://logo.com/logo.png",
                Website: "https://empresa.com"
            );

            var empresa = BuildEmpresa(1, "Empresa Nova");

            _repositoryMock
                .Setup(x => x.AdicionarAsync(It.IsAny<EmpresaEntity>()))
                .ReturnsAsync(empresa);

            // Act
            var resultado = await _useCase.AdicionarEmpresaAsync(dto);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("Empresa Nova", resultado.Value!.Nome);
        }

        [Fact(DisplayName = "EditarEmpresaAsync - Edita empresa com sucesso")]
        [Trait("UseCase", "Empresa")]
        public async Task Editar_DeveEditarEmpresa()
        {
            // Arrange
            var dto = new EmpresaDto(
                Nome: "Empresa Editada",
                Descricao: "Nova descrição",
                Area: "RH",
                ContratandoAgora: "N",
                LogoUrl: null,
                Website: null
            );

            var empresaEditada = BuildEmpresa(1, "Empresa Editada", "RH", "N");

            _repositoryMock
                .Setup(x => x.EditarAsync(1, It.IsAny<EmpresaEntity>()))
                .ReturnsAsync(empresaEditada);

            // Act
            var resultado = await _useCase.EditarEmpresaAsync(1, dto);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("Empresa Editada", resultado.Value!.Nome);
            Assert.Equal("RH", resultado.Value.Area);
        }

        [Fact(DisplayName = "EditarEmpresaAsync - Retorna NotFound quando empresa não existir")]
        [Trait("UseCase", "Empresa")]
        public async Task Editar_DeveRetornarNotFound_QuandoNaoExistir()
        {
            // Arrange
            var dto = new EmpresaDto(
                Nome: "Empresa Inexistente",
                Descricao: "Desc",
                Area: "TI",
                ContratandoAgora: "Y",
                LogoUrl: null,
                Website: null
            );

            _repositoryMock
                .Setup(x => x.EditarAsync(999, It.IsAny<EmpresaEntity>()))
                .ReturnsAsync((EmpresaEntity?)null);

            // Act
            var resultado = await _useCase.EditarEmpresaAsync(999, dto);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)resultado.StatusCode);
            Assert.Null(resultado.Value);
        }

        [Fact(DisplayName = "ObterEmpresasPorAreaAsync - Retorna empresas da área")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterPorArea_DeveRetornarEmpresas()
        {
            // Arrange
            var empresas = new[]
            {
                BuildEmpresa(1, "Empresa TI 1", "TI"),
                BuildEmpresa(2, "Empresa TI 2", "TI")
            };

            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: empresas
            );

            _repositoryMock
                .Setup(x => x.ObterPorAreaAsync("TI", 1, 10))
                .ReturnsAsync(page);

            // Act
            var resultado = await _useCase.ObterEmpresasPorAreaAsync("TI", 1, 10);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal(2, resultado.Value!.TotalRegistros);
        }

        [Fact(DisplayName = "ObterEmpresasPorAreaAsync - Retorna NoContent quando não há empresas na área")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterPorArea_DeveRetornarNoContent_QuandoNaoHaEmpresas()
        {
            // Arrange
            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 0,
                totalRegistros: 0,
                itens: Enumerable.Empty<EmpresaEntity>()
            );

            _repositoryMock
                .Setup(x => x.ObterPorAreaAsync("TI", 1, 10))
                .ReturnsAsync(page);

            // Act
            var resultado = await _useCase.ObterEmpresasPorAreaAsync("TI", 1, 10);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)resultado.StatusCode);
            Assert.Null(resultado.Value);
        }

        [Fact(DisplayName = "ObterEmpresasContratandoAsync - Retorna empresas contratando agora")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterContratando_DeveRetornarEmpresas()
        {
            // Arrange
            var empresas = new[]
            {
                BuildEmpresa(1, "Empresa 1", contratandoAgora: "Y"),
                BuildEmpresa(2, "Empresa 2", contratandoAgora: "Y")
            };

            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 1,
                totalRegistros: 2,
                itens: empresas
            );

            _repositoryMock
                .Setup(x => x.ObterContratandoAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var resultado = await _useCase.ObterEmpresasContratandoAsync(1, 10);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal(2, resultado.Value!.TotalRegistros);
        }

        [Fact(DisplayName = "ObterEmpresasContratandoAsync - Retorna NoContent quando não há empresas contratando")]
        [Trait("UseCase", "Empresa")]
        public async Task ObterContratando_DeveRetornarNoContent_QuandoNaoHaEmpresas()
        {
            // Arrange
            var page = new PageData<EmpresaEntity>(
                paginaAtual: 1,
                totalPaginas: 0,
                totalRegistros: 0,
                itens: Enumerable.Empty<EmpresaEntity>()
            );

            _repositoryMock
                .Setup(x => x.ObterContratandoAsync(1, 10))
                .ReturnsAsync(page);

            // Act
            var resultado = await _useCase.ObterEmpresasContratandoAsync(1, 10);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, (HttpStatusCode)resultado.StatusCode);
            Assert.Null(resultado.Value);
        }

        [Fact(DisplayName = "DeletarEmpresaAsync - Deleta empresa com sucesso")]
        [Trait("UseCase", "Empresa")]
        public async Task Deletar_DeveDeletarEmpresa()
        {
            // Arrange
            var empresa = BuildEmpresa(1, "Empresa 1");

            _repositoryMock
                .Setup(x => x.DeletarAsync(1))
                .ReturnsAsync(empresa);

            // Act
            var resultado = await _useCase.DeletarEmpresaAsync(1);

            // Assert
            Assert.True(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)resultado.StatusCode);
            Assert.NotNull(resultado.Value);
            Assert.Equal("Empresa 1", resultado.Value!.Nome);
        }

        [Fact(DisplayName = "DeletarEmpresaAsync - Retorna NotFound quando empresa não existir")]
        [Trait("UseCase", "Empresa")]
        public async Task Deletar_DeveRetornarNotFound_QuandoNaoExistir()
        {
            // Arrange
            _repositoryMock
                .Setup(x => x.DeletarAsync(999))
                .ReturnsAsync((EmpresaEntity?)null);

            // Act
            var resultado = await _useCase.DeletarEmpresaAsync(999);

            // Assert
            Assert.False(resultado.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, (HttpStatusCode)resultado.StatusCode);
            Assert.Null(resultado.Value);
        }
    }
}
