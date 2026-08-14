using System.ComponentModel.DataAnnotations;

namespace CMPay.Domain.Entities
{
    public class Endereco
    {
        [Key]
        public int IDEndereco { get; set; }
        public int IDCliente { get; set; }
        public required string Logradouro { get; set; }
        public required string Numero { get; set; }
        public string? Complemento { get; set; }
        public required string Bairro { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string Cep { get; set; }
        public required string Pais { get; set; }
        public Cliente Cliente { get; set; } = null!;
    }
}
