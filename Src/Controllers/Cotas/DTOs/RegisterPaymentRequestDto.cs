using System.ComponentModel.DataAnnotations;

public class RegisterPaymentRequestDto
{
    [Required(ErrorMessage = "O valor pago é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor pago deve ser maior que zero")]
    public decimal ValorPago { get; set; }
}
