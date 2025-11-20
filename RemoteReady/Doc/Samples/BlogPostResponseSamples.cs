using RemoteReady.Dtos;
using RemoteReady.Models;
using Swashbuckle.AspNetCore.Filters;

namespace RemoteReady.Doc.Samples
{
    public class BlogPostResponseListSample : IExamplesProvider<IEnumerable<BlogPostEntity>>
    {
        public IEnumerable<BlogPostEntity> GetExamples()
        {
            return new List<BlogPostEntity>
            {
                new BlogPostEntity
                {
                    Id = 1,
                    Titulo = "Como Estruturar sua Rotina de Trabalho Remoto",
                    Descricao = "Aprenda técnicas comprovadas para criar uma rotina produtiva trabalhando de casa, incluindo gestão de tempo, ambiente e pausas estratégicas.",
                    ImageUrl = "https://example.com/images/rotina-remoto.jpg",
                    Tag = "Produtividade",
                    DataCriacao = DateTime.Now.AddDays(-5)
                },
                new BlogPostEntity
                {
                    Id = 2,
                    Titulo = "Ferramentas Essenciais para Equipes Remotas",
                    Descricao = "Descubra as melhores ferramentas de comunicação, gestão de projetos e colaboração para times distribuídos geograficamente.",
                    ImageUrl = "https://example.com/images/ferramentas.jpg",
                    Tag = "Tecnologia",
                    DataCriacao = DateTime.Now.AddDays(-3)
                },
                new BlogPostEntity
                {
                    Id = 3,
                    Titulo = "Saúde Mental no Trabalho Remoto",
                    Descricao = "Entenda os desafios psicológicos do home office e estratégias para manter o equilíbrio entre vida pessoal e profissional.",
                    ImageUrl = "https://example.com/images/saude-mental.jpg",
                    Tag = "Bem-estar",
                    DataCriacao = DateTime.Now.AddDays(-2)
                },
                new BlogPostEntity
                {
                    Id = 4,
                    Titulo = "Comunicação Assíncrona: Guia Completo",
                    Descricao = "Domine a arte da comunicação assíncrona e aprenda a trabalhar efetivamente com equipes em diferentes fusos horários.",
                    ImageUrl = "https://example.com/images/comunicacao-assincrona.jpg",
                    Tag = "Comunicação",
                    DataCriacao = DateTime.Now.AddDays(-1)
                },
                new BlogPostEntity
                {
                    Id = 5,
                    Titulo = "Configurando seu Home Office Ideal",
                    Descricao = "Guia prático para montar um espaço de trabalho ergonômico e produtivo em casa, desde mobília até iluminação.",
                    ImageUrl = "https://example.com/images/home-office.jpg",
                    Tag = "Produtividade",
                    DataCriacao = DateTime.Now
                }
            };
        }
    }

    public class BlogPostResponseSample : IExamplesProvider<BlogPostEntity>
    {
        public BlogPostEntity GetExamples()
        {
            return new BlogPostEntity
            {
                Id = 1,
                Titulo = "Como Estruturar sua Rotina de Trabalho Remoto",
                Descricao = "Aprenda técnicas comprovadas para criar uma rotina produtiva trabalhando de casa, incluindo gestão de tempo, ambiente e pausas estratégicas.",
                ImageUrl = "https://example.com/images/rotina-remoto.jpg",
                Tag = "Produtividade",
                DataCriacao = DateTime.Now
            };
        }
    }

    public class BlogPostRequestSample : IExamplesProvider<BlogPostDto>
    {
        public BlogPostDto GetExamples()
        {
            return new BlogPostDto(
                "10 Dicas para Melhorar sua Produtividade Remota",                    // Titulo
                "Descubra estratégias práticas utilizadas por profissionais remotos de sucesso para maximizar sua produtividade diária.", // Descricao
                "https://example.com/images/produtividade-remota.jpg",                // ImageUrl
                "Produtividade"                                                       // Tag
            );
        }
    }

    public class BlogPostRequestUpdateSample : IExamplesProvider<BlogPostDto>
    {
        public BlogPostDto GetExamples()
        {
            return new BlogPostDto(
                "Como Estruturar sua Rotina de Trabalho Remoto - Guia 2025",          // Titulo atualizado
                "Aprenda técnicas comprovadas e atualizadas para criar uma rotina produtiva trabalhando de casa, com novos insights de 2025.", // Descricao atualizada
                "https://example.com/images/rotina-remoto-2025.jpg",                  // ImageUrl atualizada
                "Produtividade"                                                       // Tag
            );
        }
    }

    public class BlogPostRequestInvalidSample : IExamplesProvider<BlogPostDto>
    {
        public BlogPostDto GetExamples()
        {
            return new BlogPostDto(
                "",        // Titulo vazio (inválido)
                null,      // Descricao nula
                null,      // ImageUrl nula
                null       // Tag nula
            );
        }
    }
}