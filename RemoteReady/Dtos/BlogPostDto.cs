namespace RemoteReady.Dtos
{
    public record BlogPostDto(
        string Titulo,
        string? Descricao,
        string? ImageUrl,
        string? Tag
    );
}