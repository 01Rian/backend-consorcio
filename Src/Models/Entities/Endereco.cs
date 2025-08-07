using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace rian_p01_back.src.Models.Entities
{
    [Owned]
    public class Endereco
    {
        [StringLength(200)]
        public string? Logradouro { get; set; }

        [StringLength(10)]
        public string? Numero { get; set; }

        [StringLength(100)]
        public string? Complemento { get; set; }

        [StringLength(100)]
        public string? Bairro { get; set; }

        [StringLength(50)]
        public string? Cidade { get; set; }

        [StringLength(2)]
        public string? Estado { get; set; }

        [StringLength(10)]
        public string? CEP { get; set; }

        public bool IsEmpty => string.IsNullOrWhiteSpace(Logradouro) && 
                               string.IsNullOrWhiteSpace(Cidade) && 
                               string.IsNullOrWhiteSpace(CEP);
    }
}
