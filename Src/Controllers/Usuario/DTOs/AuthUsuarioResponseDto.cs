using rian_p01_back.src.Models.Entities;

public class AuthUsuarioResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public AuthUsuarioResponseDto(Usuario usuario, string token)
    {
        Id = usuario.Id;
        Email = usuario.Email;
        Nome = usuario.Nome;
        Token = token;
    }
}