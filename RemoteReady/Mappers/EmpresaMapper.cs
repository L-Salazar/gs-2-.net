using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Mappers
{
    public static class EmpresaMapper
    {
        public static EmpresaEntity ToEmpresaEntity(this EmpresaDto obj)
        {
            return new EmpresaEntity
            {
                Nome = obj.Nome,
                Descricao = obj.Descricao,
                Area = obj.Area,
                ContratandoAgora = obj.ContratandoAgora,
                LogoUrl = obj.LogoUrl,
                Website = obj.Website
            };
        }

        public static EmpresaDto ToEmpresaDto(this EmpresaEntity obj)
        {
            return new EmpresaDto(
                obj.Nome,
                obj.Descricao,
                obj.Area,
                obj.ContratandoAgora,
                obj.LogoUrl,
                obj.Website
            );
        }
    }
}