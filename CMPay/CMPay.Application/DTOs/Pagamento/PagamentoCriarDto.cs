using CMPay.Domain.Enums;

namespace CMPay.Application.DTOs
{
    public class PagamentoCriarDto
    {
        public int IDCliente { get; set; }
        public decimal ValorBruto { get; set; }
        public TipoMoeda Moeda { get; set; }
        public TipoMetodoPagamento TipoMetodoPagamento { get; set; }
    }
}
