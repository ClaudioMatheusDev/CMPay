using CMPay.Domain.Entities;

namespace CMPay.Application.Interfaces
{
    public interface IPagamentoRepository
    {
        Task<Pagamento?> BuscarPorIDAsync(int IDPagamento);
        Task<List<Pagamento>> BuscarTodosAsync();
        Task AdicionarAsync(Pagamento pagamento);
        Task SalvarAlteracoesAsync();
    }
}
