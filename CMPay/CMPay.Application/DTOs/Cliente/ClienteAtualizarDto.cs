using CMPay.Application.Validation;

namespace CMPay.Application.DTOs
{
    public class ClienteAtualizarDto
    {
        public required string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public required string Email { get; set; }
        [CpfCnpj]
        public required string Documento { get; set; }
        public required string Telefone { get; set; }
    }
}
