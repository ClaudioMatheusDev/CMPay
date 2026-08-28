using CMPay.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMPay.Application.DTOs
{
    public class PagamentoCriarDto
    {
        public int IDCliente { get; set; }
        public decimal ValorBruto { get; set; }
        [EnumDataType(typeof(TipoMoeda))]
        public TipoMoeda Moeda { get; set; }
        [EnumDataType(typeof(TipoMetodoPagamento))]
        public TipoMetodoPagamento TipoMetodoPagamento { get; set; }
    }
}
