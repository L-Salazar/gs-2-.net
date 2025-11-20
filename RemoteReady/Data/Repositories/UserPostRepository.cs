using Microsoft.EntityFrameworkCore;
using RemoteReady.Data.Repositories.Interfaces;
using RemoteReady.Models;
using RemoteReady.Data.AppData;

namespace RemoteReady.Data.Repositories
{
    public class UserPostRepository : IUserPostRepository
    {
        private readonly ApplicationContext _context;

        public UserPostRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<UserPostEntity?> AdicionarAsync(UserPostEntity entity)
        {
            _context.UserPosts.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<UserPostEntity?> DeletarAsync(int Id)
        {
            var result = await _context.UserPosts.FindAsync(Id);

            if (result is not null)
            {
                _context.UserPosts.Remove(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<UserPostEntity?> EditarAsync(int Id, UserPostEntity entity)
        {
            var result = await _context.UserPosts
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (result is not null)
            {
                result.Status = entity.Status;

                _context.UserPosts.Update(result);
                await _context.SaveChangesAsync();
                return result;
            }

            return null;
        }

        public async Task<PageData<UserPostEntity>> ObterTodosAsync(int PaginaAtual = 1, int LimitePagina = 10)
        {
            var totalRegistros = await _context.UserPosts.CountAsync();

            var result = await _context.UserPosts
                .Include(up => up.Usuario)
                .Include(up => up.Post)
                .OrderByDescending(x => x.DataLeitura)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<UserPostEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }

        public async Task<UserPostEntity?> ObterUmAsync(int Id)
        {
            var result = await _context.UserPosts
                .Include(up => up.Usuario)
                .Include(up => up.Post)
                .FirstOrDefaultAsync(x => x.Id == Id);

            return result;
        }

        public async Task<int> ObterTotalAsync()
        {
            return await _context.UserPosts.CountAsync();
        }

        public async Task<PageData<UserPostEntity>> ObterPorUsuarioAsync(int idUsuario, int PaginaAtual = 1, int LimitePagina = 10)
        {
            var query = _context.UserPosts
                .Include(up => up.Post)
                .Where(x => x.IdUsuario == idUsuario);

            var totalRegistros = await query.CountAsync();

            var result = await query
                .OrderByDescending(x => x.DataLeitura)
                .Skip((PaginaAtual - 1) * LimitePagina)
                .Take(LimitePagina)
                .ToListAsync();

            var totalPaginas = (totalRegistros + LimitePagina - 1) / LimitePagina;

            return new PageData<UserPostEntity>(PaginaAtual, totalPaginas, totalRegistros, result);
        }

        public async Task<int> ObterTotalPostsLidosPorUsuarioAsync(int idUsuario)
        {
            return await _context.UserPosts
                .Where(x => x.IdUsuario == idUsuario && x.Status == "LIDO")
                .CountAsync();
        }

        public async Task<bool> UsuarioJaLeuPostAsync(int idUsuario, int idPost)
        {
            var entidade = await _context.UserPosts
        .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario && x.IdPost == idPost);

            return entidade != null;
        }

        public async Task<UserPostEntity?> ObterPorUsuarioEPostAsync(int idUsuario, int idPost)
        {
            return await _context.UserPosts
                .Include(up => up.Usuario)
                .Include(up => up.Post)
                .FirstOrDefaultAsync(x => x.IdUsuario == idUsuario && x.IdPost == idPost);
        }
    }
}