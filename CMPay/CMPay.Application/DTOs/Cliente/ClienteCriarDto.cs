using CMPay.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace CMPay.Application.DTOs
{
    public class ClienteCriarDto
    {
        public required string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [CpfCnpj]
        public required string Documento { get; set; }
        [Phone]
        public required string Telefone { get; set; }
    }
}
