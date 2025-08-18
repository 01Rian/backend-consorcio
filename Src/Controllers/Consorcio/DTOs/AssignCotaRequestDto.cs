using System.ComponentModel.DataAnnotations;

public class AssignCotaRequestDto
{
    [Required(ErrorMessage = "CotaId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "CotaId deve ser maior que zero")]
    public int CotaId { get; set; }

    [Required(ErrorMessage = "UsuarioId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "UsuarioId deve ser maior que zero")]
    public int UsuarioId { get; set; }
}