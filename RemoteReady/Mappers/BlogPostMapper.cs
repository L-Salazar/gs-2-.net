using RemoteReady.Dtos;
using RemoteReady.Models;

namespace RemoteReady.Mappers
{
    public static class BlogPostMapper
    {
        public static BlogPostEntity ToBlogPostEntity(this BlogPostDto obj)
        {
            return new BlogPostEntity
            {
                Titulo = obj.Titulo,
                Descricao = obj.Descricao,
                ImageUrl = obj.ImageUrl,
                Tag = obj.Tag
            };
        }

        public static BlogPostDto ToBlogPostDto(this BlogPostEntity obj)
        {
            return new BlogPostDto(
                obj.Titulo,
                obj.Descricao,
                obj.ImageUrl,
                obj.Tag
            );
        }
    }
}