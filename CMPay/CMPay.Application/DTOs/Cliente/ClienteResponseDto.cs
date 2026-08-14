namespace CMPay.Application.DTOs
{
    public class ClienteResponseDto
    {
        public int IDCliente { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required string Documento { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
