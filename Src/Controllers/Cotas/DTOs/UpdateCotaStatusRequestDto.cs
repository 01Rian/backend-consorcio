using System.ComponentModel.DataAnnotations;

public class UpdateCotaStatusRequestDto
{
    [Required(ErrorMessage = "O Status é obrigatório")]
    public StatusCota Status { get; set; }
}
