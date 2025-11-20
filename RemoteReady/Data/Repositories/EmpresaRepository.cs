using Microsoft.EntityFrameworkCore;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Models;
using RemoteReady.Data.AppData;

namespace RemoteReady.Data.Repositories
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly ApplicationContext _context;

        public EmpresaRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<EmpresaEntity?> AdicionarAsync(EmpresaEntity entity)
        {
            _context.Empresas.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<EmpresaEntity?> DeletarAsync(int Id)
        {
            var result = await _context.Empresas.FindAsync(Id);

            if (result is not null)
            {
                _context.Empresas.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<EmpresaEntity?> EditarAsync(int Id, EmpresaEntity entity)
        {
            var result = await _context.Empresas
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (result is not null)
            {
                result.Nome = entity.Nome;
                result.Descricao = entity.Descricao;
                result.Area = entity.Area;
                result.ContratandoAgora = entity.ContratandoAgora;
                result.LogoUrl = entity.LogoUrl;
                result.Website = entity.Website;

                _context.Empresas.Update(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<PageData<EmpresaEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var totalRegistros = await _context.Empresas.CountAsync();

            var result = await _context.Empresas
                .OrderBy(x => x.Id)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<EmpresaEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }

        public async Task<EmpresaEntity?> ObterUmaAsync(int Id)
        {
            var result = await _context.Empresas
                .FirstOrDefaultAsync(x => x.Id == Id);

            return result;
        }

        public async Task<int> ObterTotalAsync()
        {
            return await _context.Empresas.CountAsync();
        }

        public async Task<PageData<EmpresaEntity>> ObterPorAreaAsync(string area, int PaginaAtual = 1, int LimitePagina = 10)
        {
            var query = _context.Empresas
                .Where(x => x.Area != null && x.Area.ToLower().Contains(area.ToLower()));

            var totalRegistros = await query.CountAsync();

            var result = await query
                .OrderBy(x => x.Id)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<EmpresaEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }

        public async Task<PageData<EmpresaEntity>> ObterContratandoAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var query = _context.Empresas
                .Where(x => x.ContratandoAgora == "Y");

            var totalRegistros = await query.CountAsync();

            var result = await query
                .OrderBy(x => x.Id)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<EmpresaEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }
    }
}