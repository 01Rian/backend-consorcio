using System.ComponentModel.DataAnnotations;

namespace rian_p01_back.src.Models.Entities
{
    public class Administradora
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(18)]
        public string CNPJ { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Telefone { get; set; }

        [StringLength(100)]
        public string? Email { get; set; }

        public Endereco? Endereco { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public virtual ICollection<Consorcio> Consorcios { get; set; } = new List<Consorcio>();
    }
}
