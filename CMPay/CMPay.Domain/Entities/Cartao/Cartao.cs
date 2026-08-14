using CMPay.Domain.Enums.Cartao;
using System.ComponentModel.DataAnnotations;

namespace CMPay.Domain.Entities
{
    public class Cartao
    {
        [Key]
        public int IDCartao { get; set; }
        public int IDCliente { get; set; }
        public BandeiraCartao BandeiraCartao { get; set; }
        public required string UltimosDigitos { get; set; }
        public int MesExpiracao { get; set; }
        public int AnoExpiracao { get; set; }
        public required string NomeTitular { get; set; }
        public bool Padrao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Cliente Cliente { get; set; } = null!;
    }
}
