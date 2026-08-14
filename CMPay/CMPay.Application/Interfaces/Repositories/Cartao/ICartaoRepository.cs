using CMPay.Domain.Entities;

namespace CMPay.Application.Interfaces
{
    public interface ICartaoRepository
    {
        Task<Cartao?> BuscarCartaoPorIDAsync(int IDCartao);
        Task<Cartao?> BuscarCartaoPorCliente(int IDCliente);
        Task<List<Cartao>> BuscarTodosCartoes();

        Task AdicionarCartaoAsync(Cartao cartao);
        void Atualizar(Cartao cartao);
        void Remover(Cartao cartao);

        Task SalvarAlteracoesAsync();
    }
}
