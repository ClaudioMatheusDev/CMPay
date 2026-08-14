using CMPay.Domain.Enums;

namespace CMPay.Application.DTOs
{
    public class TransacaoResponseDto
    {
        public int IDTransacao { get; set; }
        public int IDPagamento { get; set; }
        public TipoTransacao TipoTransacao { get; set; }
        public StatusTransacao StatusTransacao { get; set; }
        public string? Mensagem { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransacao { get; set; }
    }
}
