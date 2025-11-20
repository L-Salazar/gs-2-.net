using RemoteReady.Models;
using Microsoft.EntityFrameworkCore;

namespace RemoteReady.Data.AppData
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        public DbSet<UsuarioEntity> Usuarios { get; set; }
        public DbSet<EmpresaEntity> Empresas { get; set; }
        public DbSet<BlogPostEntity> BlogPosts { get; set; }
        public DbSet<UserPostEntity> UserPosts { get; set; }
    }
}