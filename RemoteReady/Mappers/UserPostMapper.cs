using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Mappers
{
    public static class UserPostMapper
    {
        public static UserPostEntity ToUserPostEntity(this UserPostDto obj)
        {
            return new UserPostEntity
            {
                IdUsuario = obj.IdUsuario,
                IdPost = obj.IdPost,
                Status = obj.Status
            };
        }

        public static UserPostDto ToUserPostDto(this UserPostEntity obj)
        {
            return new UserPostDto(
                obj.IdUsuario,
                obj.IdPost,
                obj.Status
            );
        }
    }
}