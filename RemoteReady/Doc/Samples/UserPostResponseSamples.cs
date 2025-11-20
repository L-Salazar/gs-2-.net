using RemoteReady.Dtos;
using RemoteReady.Models;
using Swashbuckle.AspNetCore.Filters;

namespace RemoteReady.Doc.Samples
{
    public class UserPostResponseListSample : IExamplesProvider<IEnumerable<UserPostEntity>>
    {
        public IEnumerable<UserPostEntity> GetExamples()
        {
            return new List<UserPostEntity>
            {
                new UserPostEntity
                {
                    Id = 1,
                    IdUsuario = 1,
                    IdPost = 1,
                    Status = "LIDO",
                    DataLeitura = DateTime.Now.AddDays(-5),
                    Usuario = new UsuarioEntity
                    {
                        Id = 1,
                        Nome = "João Silva",
                        Email = "joao@email.com"
                    },
                    Post = new BlogPostEntity
                    {
                        Id = 1,
                        Titulo = "Como Estruturar sua Rotina de Trabalho Remoto"
                    }
                },
                new UserPostEntity
                {
                    Id = 2,
                    IdUsuario = 1,
                    IdPost = 2,
                    Status = "LIDO",
                    DataLeitura = DateTime.Now.AddDays(-3),
                    Usuario = new UsuarioEntity
                    {
                        Id = 1,
                        Nome = "João Silva",
                        Email = "joao@email.com"
                    },
                    Post = new BlogPostEntity
                    {
                        Id = 2,
                        Titulo = "Ferramentas Essenciais para Equipes Remotas"
                    }
                },
                new UserPostEntity
                {
                    Id = 3,
                    IdUsuario = 2,
                    IdPost = 1,
                    Status = "LIDO",
                    DataLeitura = DateTime.Now.AddDays(-1),
                    Usuario = new UsuarioEntity
                    {
                        Id = 2,
                        Nome = "Maria Santos",
                        Email = "maria@email.com"
                    },
                    Post = new BlogPostEntity
                    {
                        Id = 1,
                        Titulo = "Como Estruturar sua Rotina de Trabalho Remoto"
                    }
                }
            };
        }
    }

    public class UserPostResponseSample : IExamplesProvider<UserPostEntity>
    {
        public UserPostEntity GetExamples()
        {
            return new UserPostEntity
            {
                Id = 1,
                IdUsuario = 1,
                IdPost = 1,
                Status = "LIDO",
                DataLeitura = DateTime.Now,
                Usuario = new UsuarioEntity
                {
                    Id = 1,
                    Nome = "João Silva",
                    Email = "joao@email.com"
                },
                Post = new BlogPostEntity
                {
                    Id = 1,
                    Titulo = "Como Estruturar sua Rotina de Trabalho Remoto"
                }
            };
        }
    }

    public class UserPostRequestSample : IExamplesProvider<UserPostDto>
    {
        public UserPostDto GetExamples()
        {
            return new UserPostDto(
                1,          // IdUsuario
                1,          // IdPost
                "LIDO"      // Status
            );
        }
    }

    public class UserPostRequestUpdateSample : IExamplesProvider<UserPostDto>
    {
        public UserPostDto GetExamples()
        {
            return new UserPostDto(
                1,              // IdUsuario
                1,              // IdPost
                "EM_ANDAMENTO"  // Status atualizado
            );
        }
    }

    public class ProgressoUsuarioResponseSample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                IdUsuario = 1,
                TotalPostsLidos = 12,
                PostsNecessariosParaCertificado = 10,
                PostsFaltantes = 0,
                ElegivelParaCertificado = true,
                PercentualConcluido = 120.0,
                Mensagem = "Parabéns! Você já pode gerar seu certificado de conclusão!"
            };
        }
    }

    public class ProgressoUsuarioIncompletoSample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            return new
            {
                IdUsuario = 2,
                TotalPostsLidos = 7,
                PostsNecessariosParaCertificado = 10,
                PostsFaltantes = 3,
                ElegivelParaCertificado = false,
                PercentualConcluido = 70.0,
                Mensagem = "Continue lendo! Faltam apenas 3 posts para você gerar seu certificado."
            };
        }
    }
}