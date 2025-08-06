using System.ComponentModel.DataAnnotations;

namespace rian_p01_back.src.Models.Entities
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [StringLength(14)]
        public string CPF { get; set; } = string.Empty;

        [StringLength(15)]
        public string? Telefone { get; set; }

        [StringLength(15)]
        public string? Celular { get; set; }

        public DateTime? DataNascimento { get; set; }

        public Endereco? Endereco { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public DateTime? DataAtualizacao { get; set; }

        public virtual ICollection<Cotas> Cotas { get; set; } = new List<Cotas>();
    }
}
