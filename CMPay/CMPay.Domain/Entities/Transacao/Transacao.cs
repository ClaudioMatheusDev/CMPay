using CMPay.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMPay.Domain.Entities
{
    public class Transacao
    {
        [Key]
        public int IDTransacao { get; set; }
        public int IDPagamento { get; set; }
        public TipoTransacao TipoTransacao { get; set; }
        public StatusTransacao StatusTransacao { get; set; }
        public string? Mensagem { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataTransacao { get; set; } = DateTime.UtcNow;
        public Pagamento Pagamento { get; set; } = null!;
    }
}
