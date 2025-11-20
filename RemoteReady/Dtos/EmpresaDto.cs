namespace RemoteReady.Dtos
{
    public record EmpresaDto(
        string Nome,
        string? Descricao,
        string? Area,
        string ContratandoAgora,
        string? LogoUrl,
        string? Website
    );
}