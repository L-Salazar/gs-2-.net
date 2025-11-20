namespace RemoteReady.Dtos
{
    public record UsuarioDto(
        string Nome,
        string Email,
        string Senha,
        string TipoUsuario
    );
}