using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rian_p01_back.src.Models.Entities
{
    public class Cotas
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string NumeroCota { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor da parcela deve ser maior que zero.")]
        public decimal ValorParcela { get; set; }

        public decimal ValorPago { get; set; } = 0;

        public int ParcelasPagas { get; set; } = 0;

        public bool Contemplada { get; set; } = false;

        public DateTime? DataContemplacao { get; set; }

        [Required]
        public StatusCota Status { get; set; } = StatusCota.Ativo;

        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "O ConsorcioId deve ser maior que zero.")]
        public int ConsorcioId { get; set; }

        public int? UsuarioId { get; set; }

        [ForeignKey("ConsorcioId")]
        public virtual Consorcio Consorcio { get; set; } = null!;

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }
    }
}
