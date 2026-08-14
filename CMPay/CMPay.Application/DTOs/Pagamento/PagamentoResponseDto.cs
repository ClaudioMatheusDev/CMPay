using CMPay.Domain.Enums;

namespace CMPay.Application.DTOs
{
    public class PagamentoResponseDto
    {
        public int IDPagamento { get; set; }
        public int IDCliente { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorTaxa { get; set; }
        public decimal ValorLiquido { get; set; }
        public TipoMoeda Moeda { get; set; }
        public TipoMetodoPagamento TipoMetodoPagamento { get; set; }
        public StatusPagamento StatusPagamento { get; set; }
        public DateTime DataCriacao { get; set; } 
        public DateTime? DataPagamento { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public DateTime? DataEstorno { get; set; }

    }
}
