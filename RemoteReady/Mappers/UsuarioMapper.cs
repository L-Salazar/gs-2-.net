using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Mappers
{
    public static class UsuarioMapper
    {
        public static UsuarioEntity ToUsuarioEntity(this UsuarioDto obj)
        {
            return new UsuarioEntity
            {
                Nome = obj.Nome,
                Email = obj.Email,
                Senha = obj.Senha,
                TipoUsuario = obj.TipoUsuario
            };
        }

        public static UsuarioDto ToUsuarioDto(this UsuarioEntity obj)
        {
            return new UsuarioDto(
                obj.Nome,
                obj.Email,
                obj.Senha,
                obj.TipoUsuario
            );
        }
    }
}