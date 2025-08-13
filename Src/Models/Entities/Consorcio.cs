using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rian_p01_back.src.Models.Entities
{
    public class Consorcio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public decimal ValorBem { get; set; }

        [Required]
        public int QuantidadeCotas { get; set; }

        [Required]
        public int PrazoMeses { get; set; }

        [Required]
        public decimal TaxaAdministracao { get; set; }

        [Required]
        public decimal FundoReserva { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataTermino { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public virtual ICollection<Cotas> Cotas { get; set; } = new List<Cotas>();
    }
}
