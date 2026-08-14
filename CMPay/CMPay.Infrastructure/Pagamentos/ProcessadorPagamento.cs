using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;

namespace CMPay.Infrastructure.Pagamentos
{
    public class ProcessadorPagamento : IProcessadorPagamento
    {
        public Task<ProcessamentoPagamentoResultadoDto> ProcessarAsync(
            Pagamento pagamento)
        {
            if (pagamento.ValorBruto <= 1000)
            {
                return Task.FromResult(
                    new ProcessamentoPagamentoResultadoDto
                    {
                        Aprovado = true,
                        Mensagem = "Pagamento aprovado."
                    });
            }

            return Task.FromResult(
                new ProcessamentoPagamentoResultadoDto
                {
                    Aprovado = false,
                    Mensagem = "Pagamento reprovado."
                });
        }
    }
}
