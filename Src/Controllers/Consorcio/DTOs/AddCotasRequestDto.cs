using System.ComponentModel.DataAnnotations;

public class AddCotasRequestDto
{
    [Required(ErrorMessage = "Quantidade de cotas é obrigatória")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantidade de cotas deve ser maior que zero")]
    public int Quantidade { get; set; }
}