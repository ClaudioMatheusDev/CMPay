using CMPay.Domain.Enums.Cartao;
using System.ComponentModel.DataAnnotations;

namespace CMPay.Application.DTOs
{
    public class CartaoAtualizarDto : IValidatableObject
    {
        public int IDCliente { get; set; }
        public BandeiraCartao BandeiraCartao { get; set; }
        public required string UltimosDigitos { get; set; }
        [Range(1,12)]
        public int MesExpiracao { get; set; }
        [Range(2000,2100)]
        public int AnoExpiracao { get; set; }
        public required string NomeTitular { get; set; }
        public bool Padrao { get; set; }
        public bool Ativo { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MesExpiracao < 1 || MesExpiracao > 12)
                yield break;

            var expiraEm = new DateTime(AnoExpiracao, MesExpiracao, 1).AddMonths(1);

            if (expiraEm <= DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Cartão expirado.",
                    new[] { nameof(MesExpiracao), nameof(AnoExpiracao) });
            }
        }
    }
}
