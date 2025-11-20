using Microsoft.EntityFrameworkCore;
using RemoteReady.Data.AppData;
using RemoteReady.Data.Repositories;
using RemoteReady.Models;
using Xunit;

namespace RemoteReady.Tests.Repositories
{
    public class EmpresaRepositoryTest
    {
        private ApplicationContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        private static EmpresaEntity BuildEmpresa(
            int id = 0,
            string nome = "Empresa Teste",
            string? descricao = "Descrição teste",
            string? area = "TI",
            string contratandoAgora = "Y",
            string? logoUrl = "https://logo.com/logo.png",
            string? website = "https://empresa.com"
        )
        {
            return new EmpresaEntity
            {
                Id = id,
                Nome = nome,
                Descricao = descricao,
                Area = area,
                ContratandoAgora = contratandoAgora,
                LogoUrl = logoUrl,
                Website = website,
                DataCriacao = DateTime.UtcNow
            };
        }

        [Fact(DisplayName = "ObterTodosAsync - Retorna empresas paginadas")]
        [Trait("Repository", "Empresa")]
        public async Task ObterTodosAsync_DeveRetornarEmpresas()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            var emp1 = BuildEmpresa(nome: "Empresa 1");
            var emp2 = BuildEmpresa(nome: "Empresa 2");

            context.Empresas.AddRange(emp1, emp2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.ObterTodosAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalRegistros);
            Assert.Equal(1, result.PaginaAtual);
            Assert.Equal(2, result.Itens.Count());
        }

        [Fact(DisplayName = "ObterUmaAsync - Retorna empresa existente")]
        [Trait("Repository", "Empresa")]
        public async Task ObterUmaAsync_DeveRetornarEmpresaExistente()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            var emp = BuildEmpresa(nome: "Empresa Única");
            context.Empresas.Add(emp);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.ObterUmaAsync(emp.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Empresa Única", result!.Nome);
        }

        [Fact(DisplayName = "ObterUmaAsync - Retorna null quando não encontrar")]
        [Trait("Repository", "Empresa")]
        public async Task ObterUmaAsync_DeveRetornarNull_QuandoNaoEncontrar()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            // Act
            var result = await repository.ObterUmaAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "AdicionarAsync - Adiciona empresa com sucesso")]
        [Trait("Repository", "Empresa")]
        public async Task AdicionarAsync_DeveAdicionarEmpresa()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            var emp = BuildEmpresa(nome: "Empresa Nova");

            // Act
            var retorno = await repository.AdicionarAsync(emp);

            // Assert
            Assert.NotNull(retorno);
            Assert.True(retorno!.Id > 0);
            Assert.Equal("Empresa Nova", retorno.Nome);
        }

        [Fact(DisplayName = "EditarAsync - Edita empresa existente")]
        [Trait("Repository", "Empresa")]
        public async Task EditarAsync_DeveEditarEmpresaExistente()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            var emp = BuildEmpresa(nome: "Empresa Antiga");
            context.Empresas.Add(emp);
            await context.SaveChangesAsync();

            var nova = BuildEmpresa(nome: "Empresa Atualizada", descricao: "Nova descrição");

            // Act
            var retorno = await repository.EditarAsync(emp.Id, nova);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal("Empresa Atualizada", retorno!.Nome);
            Assert.Equal("Nova descrição", retorno.Descricao);
        }

        [Fact(DisplayName = "EditarAsync - Retorna null quando empresa não existe")]
        [Trait("Repository", "Empresa")]
        public async Task EditarAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            var nova = BuildEmpresa(nome: "Fantasma");

            // Act
            var retorno = await repository.EditarAsync(999, nova);

            // Assert
            Assert.Null(retorno);
        }

        [Fact(DisplayName = "DeletarAsync - Remove empresa existente")]
        [Trait("Repository", "Empresa")]
        public async Task DeletarAsync_DeveRemoverEmpresa()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            var emp = BuildEmpresa(nome: "Empresa para deletar");
            context.Empresas.Add(emp);
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.DeletarAsync(emp.Id);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(emp.Id, retorno!.Id);
            Assert.False(await context.Empresas.AnyAsync(x => x.Id == emp.Id));
        }

        [Fact(DisplayName = "DeletarAsync - Retorna null quando empresa não existe")]
        [Trait("Repository", "Empresa")]
        public async Task DeletarAsync_DeveRetornarNull_QuandoNaoExistir()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            // Act
            var retorno = await repository.DeletarAsync(999);

            // Assert
            Assert.Null(retorno);
        }

        [Fact(DisplayName = "ObterTotalAsync - Retorna total de empresas")]
        [Trait("Repository", "Empresa")]
        public async Task ObterTotalAsync_DeveRetornarTotal()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            context.Empresas.AddRange(
                BuildEmpresa(nome: "Empresa 1"),
                BuildEmpresa(nome: "Empresa 2"),
                BuildEmpresa(nome: "Empresa 3")
            );
            await context.SaveChangesAsync();

            // Act
            var total = await repository.ObterTotalAsync();

            // Assert
            Assert.Equal(3, total);
        }

        [Fact(DisplayName = "ObterPorAreaAsync - Retorna empresas da área")]
        [Trait("Repository", "Empresa")]
        public async Task ObterPorAreaAsync_DeveRetornarEmpresasDaArea()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            context.Empresas.AddRange(
                BuildEmpresa(nome: "Empresa TI 1", area: "TI"),
                BuildEmpresa(nome: "Empresa TI 2", area: "TI"),
                BuildEmpresa(nome: "Empresa RH", area: "RH")
            );
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.ObterPorAreaAsync("ti", 1, 10);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(2, retorno.Itens.Count());
            Assert.All(retorno.Itens, e => Assert.Equal("TI", e.Area));
        }

        [Fact(DisplayName = "ObterContratandoAsync - Retorna apenas empresas contratando agora")]
        [Trait("Repository", "Empresa")]
        public async Task ObterContratandoAsync_DeveRetornarApenasContratando()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new EmpresaRepository(context);

            context.Empresas.AddRange(
                BuildEmpresa(nome: "Empresa 1", contratandoAgora: "Y"),
                BuildEmpresa(nome: "Empresa 2", contratandoAgora: "N"),
                BuildEmpresa(nome: "Empresa 3", contratandoAgora: "Y")
            );
            await context.SaveChangesAsync();

            // Act
            var retorno = await repository.ObterContratandoAsync(1, 10);

            // Assert
            Assert.NotNull(retorno);
            Assert.Equal(2, retorno.Itens.Count());
            Assert.All(retorno.Itens, e => Assert.Equal("Y", e.ContratandoAgora));
        }
    }
}
