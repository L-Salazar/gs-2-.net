using RemoteReady.Dtos;
using RemoteReady.Models;
using Swashbuckle.AspNetCore.Filters;

namespace RemoteReady.Doc.Samples
{
    public class EmpresaResponseListSample : IExamplesProvider<IEnumerable<EmpresaEntity>>
    {
        public IEnumerable<EmpresaEntity> GetExamples()
        {
            return new List<EmpresaEntity>
            {
                new EmpresaEntity
                {
                    Id = 1,
                    Nome = "GitLab",
                    Descricao = "Plataforma de DevOps completa, 100% remoto desde o início",
                    Area = "Tecnologia",
                    ContratandoAgora = "Y",
                    LogoUrl = "https://example.com/logos/gitlab.png",
                    Website = "https://gitlab.com/careers",
                    DataCriacao = DateTime.Now
                },
                new EmpresaEntity
                {
                    Id = 2,
                    Nome = "Automattic",
                    Descricao = "Empresa por trás do WordPress, trabalho remoto distribuído globalmente",
                    Area = "Tecnologia",
                    ContratandoAgora = "Y",
                    LogoUrl = "https://example.com/logos/automattic.png",
                    Website = "https://automattic.com/work-with-us",
                    DataCriacao = DateTime.Now
                },
                new EmpresaEntity
                {
                    Id = 3,
                    Nome = "Toptal",
                    Descricao = "Rede de freelancers de elite, modelo 100% remoto",
                    Area = "Consultoria",
                    ContratandoAgora = "N",
                    LogoUrl = "https://example.com/logos/toptal.png",
                    Website = "https://www.toptal.com/careers",
                    DataCriacao = DateTime.Now
                },
                new EmpresaEntity
                {
                    Id = 4,
                    Nome = "Zapier",
                    Descricao = "Automação de workflows, equipe distribuída pelo mundo",
                    Area = "Tecnologia",
                    ContratandoAgora = "Y",
                    LogoUrl = "https://example.com/logos/zapier.png",
                    Website = "https://zapier.com/jobs",
                    DataCriacao = DateTime.Now
                }
            };
        }
    }

    public class EmpresaResponseSample : IExamplesProvider<EmpresaEntity>
    {
        public EmpresaEntity GetExamples()
        {
            return new EmpresaEntity
            {
                Id = 1,
                Nome = "GitLab",
                Descricao = "Plataforma de DevOps completa, 100% remoto desde o início",
                Area = "Tecnologia",
                ContratandoAgora = "Y",
                LogoUrl = "https://example.com/logos/gitlab.png",
                Website = "https://gitlab.com/careers",
                DataCriacao = DateTime.Now
            };
        }
    }

    public class EmpresaRequestSample : IExamplesProvider<EmpresaDto>
    {
        public EmpresaDto GetExamples()
        {
            return new EmpresaDto(
                "Buffer",                               // Nome
                "Plataforma de social media, pioneira em trabalho remoto", // Descricao
                "Marketing Digital",                    // Area
                "Y",                                    // ContratandoAgora
                "https://example.com/logos/buffer.png", // LogoUrl
                "https://buffer.com/journey"            // Website
            );
        }
    }

    public class EmpresaRequestUpdateSample : IExamplesProvider<EmpresaDto>
    {
        public EmpresaDto GetExamples()
        {
            return new EmpresaDto(
                "GitLab Inc.",                          // Nome atualizado
                "A plataforma completa de DevOps com IA, 100% remoto", // Descricao atualizada
                "Tecnologia & DevOps",                  // Area atualizada
                "N",                                    // Não está contratando
                "https://example.com/logos/gitlab-new.png", // LogoUrl atualizada
                "https://about.gitlab.com/jobs"         // Website atualizado
            );
        }
    }

    public class EmpresaRequestInvalidSample : IExamplesProvider<EmpresaDto>
    {
        public EmpresaDto GetExamples()
        {
            return new EmpresaDto(
                "",        // Nome vazio (inválido)
                null,      // Descricao nula
                null,      // Area nula
                "X",       // ContratandoAgora inválido (deve ser Y ou N)
                null,      // LogoUrl nula
                null       // Website nulo
            );
        }
    }
}