using CMPay.Domain.Enums.Cartao;

namespace CMPay.Application.DTOs
{
    public class CartaoResponseDto
    {
        public int IDCartao { get; set; }
        public int IDCliente { get; set; }
        public BandeiraCartao BandeiraCartao { get; set; }
        public required string UltimosDigitos { get; set; }
        public int MesExpiracao { get; set; }
        public int AnoExpiracao { get; set; }
        public required string NomeTitular { get; set; }
        public bool Padrao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
