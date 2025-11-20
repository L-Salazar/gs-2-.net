using Microsoft.EntityFrameworkCore;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Models;
using RemoteReady.Data.AppData;

namespace RemoteReady.Data.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationContext _context;

        public BlogPostRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<BlogPostEntity?> AdicionarAsync(BlogPostEntity entity)
        {
            _context.BlogPosts.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<BlogPostEntity?> DeletarAsync(int Id)
        {
            var result = await _context.BlogPosts.FindAsync(Id);

            if (result is not null)
            {
                _context.BlogPosts.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<BlogPostEntity?> EditarAsync(int Id, BlogPostEntity entity)
        {
            var result = await _context.BlogPosts
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (result is not null)
            {
                result.Titulo = entity.Titulo;
                result.Descricao = entity.Descricao;
                result.ImageUrl = entity.ImageUrl;
                result.Tag = entity.Tag;

                _context.BlogPosts.Update(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<PageData<BlogPostEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var totalRegistros = await _context.BlogPosts.CountAsync();

            var result = await _context.BlogPosts
                .OrderByDescending(x => x.DataCriacao)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<BlogPostEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }

        public async Task<BlogPostEntity?> ObterUmAsync(int Id)
        {
            var result = await _context.BlogPosts
                .FirstOrDefaultAsync(x => x.Id == Id);

            return result;
        }

        public async Task<int> ObterTotalAsync()
        {
            return await _context.BlogPosts.CountAsync();
        }

        public async Task<PageData<BlogPostEntity>> ObterPorTagAsync(string tag, int PaginaAtual = 1, int LimitePagina = 10)
        {
            var query = _context.BlogPosts
                .Where(x => x.Tag != null && x.Tag.ToLower().Contains(tag.ToLower()));

            var totalRegistros = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.DataCriacao)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<BlogPostEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }

        public async Task<PageData<BlogPostEntity>> ObterRecentesAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var totalRegistros = await _context.BlogPosts.CountAsync();

            var result = await _context.BlogPosts
                .OrderByDescending(x => x.DataCriacao)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<BlogPostEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }
    }
}