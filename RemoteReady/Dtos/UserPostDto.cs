namespace RemoteReady.Dtos
{
    public record UserPostDto(
        int IdUsuario,
        int IdPost,
        string Status
    );
}