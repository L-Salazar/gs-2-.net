using RemoteReady.Dtos;
using RemoteReady.Models;
using Swashbuckle.AspNetCore.Filters;

namespace RemoteReady.Doc.Samples
{
    public class UsuarioResponseListSample : IExamplesProvider<IEnumerable<UsuarioEntity>>
    {
        public IEnumerable<UsuarioEntity> GetExamples()
        {
            return new List<UsuarioEntity>
            {
                new UsuarioEntity
                {
                    Id = 1,
                    Nome = "João Silva",
                    Email = "joao.silva@email.com",
                    Senha = "senha123",
                    TipoUsuario = "USER",
                    DataCriacao = DateTime.Now
                },
                new UsuarioEntity
                {
                    Id = 2,
                    Nome = "Maria Santos",
                    Email = "maria.santos@email.com",
                    Senha = "senha456",
                    TipoUsuario = "ADMIN",
                    DataCriacao = DateTime.Now
                },
                new UsuarioEntity
                {
                    Id = 3,
                    Nome = "Pedro Costa",
                    Email = "pedro.costa@email.com",
                    Senha = "senha789",
                    TipoUsuario = "OPERADOR",
                    DataCriacao = DateTime.Now
                }
            };
        }
    }

    public class UsuarioResponseSamples : IExamplesProvider<UsuarioEntity>
    {
        public UsuarioEntity GetExamples()
        {
            return new UsuarioEntity
            {
                Id = 1,
                Nome = "João Silva",
                Email = "joao.silva@email.com",
                Senha = "senha123",
                TipoUsuario = "USER",
                DataCriacao = DateTime.Now
            };
        }
    }

    public class UsuarioRequestSample : IExamplesProvider<UsuarioDto>
    {
        public UsuarioDto GetExamples()
        {
            return new UsuarioDto(
                "João Silva",           // Nome
                "joao.silva@email.com", // Email
                "senha123",             // Senha
                "USER"                  // TipoUsuario
            );
        }
    }

    public class UsuarioRequestUpdateSample : IExamplesProvider<UsuarioDto>
    {
        public UsuarioDto GetExamples()
        {
            return new UsuarioDto(
                "João Silva Atualizado",     // Nome atualizado
                "joao.silva.novo@email.com", // Email atualizado
                "novaSenha456",              // Nova senha
                "ADMIN"                      // Tipo usuário atualizado
            );
        }
    }

    public class UsuarioRequestInvalidSample : IExamplesProvider<UsuarioDto>
    {
        public UsuarioDto GetExamples()
        {
            return new UsuarioDto(
                "",        // Nome vazio (inválido)
                "",        // Email vazio (inválido)
                "",        // Senha vazia (inválido)
                ""         // TipoUsuario vazio (inválido)
            );
        }
    }
}