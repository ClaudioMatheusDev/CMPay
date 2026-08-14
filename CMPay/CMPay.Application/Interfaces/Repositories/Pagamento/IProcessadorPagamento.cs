using CMPay.Application.DTOs;
using CMPay.Domain.Entities;

namespace CMPay.Application.Interfaces
{
    public interface IProcessadorPagamento
    {
        Task<ProcessamentoPagamentoResultadoDto> ProcessarAsync(Pagamento pagamento);
    }
}