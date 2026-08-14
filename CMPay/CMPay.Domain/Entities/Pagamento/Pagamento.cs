using CMPay.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CMPay.Domain.Entities
{
    public class Pagamento
    {
        [Key]
        public int IDPagamento { get; set; }
        public int IDCliente { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorTaxa { get; set; }
        public decimal ValorLiquido { get; set; }
        public TipoMoeda Moeda { get; set; }
        public TipoMetodoPagamento TipoMetodoPagamento { get; set;}
        public StatusPagamento StatusPagamento { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataPagamento { get; set; }
        public DateTime? DataCancelamento { get; set; }
        public DateTime? DataEstorno { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }
}
