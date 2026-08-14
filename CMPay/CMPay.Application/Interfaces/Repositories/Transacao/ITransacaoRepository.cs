using CMPay.Application.DTOs;
using CMPay.Domain.Entities;

namespace CMPay.Application.Interfaces
{
    public interface ITransacaoRepository
    {
        Task AdicionarAsync(Transacao transacao);

        Task<List<Transacao>> BuscarPorPagamentoAsync(int IDPagamento);

        Task SalvarAlteracoesAsync();
    }
}
