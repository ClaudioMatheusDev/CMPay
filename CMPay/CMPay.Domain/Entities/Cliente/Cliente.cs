using System.ComponentModel.DataAnnotations;

namespace CMPay.Domain.Entities

{
    public class Cliente
    {
        [Key]
        public int IDCliente { get; set; }
        public required string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public required string Email { get; set; }
        public required string Documento { get; set; }
        public required string Telefone { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public Endereco? Endereco { get; set; }
        public ICollection<Cartao>  Cartoes{get; set;} = new List<Cartao>();
        public ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
    }
}
