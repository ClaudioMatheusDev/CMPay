using CMPay.Application.DTOs;

namespace CMPay.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<int> CriarPagamentoAsync(PagamentoCriarDto pagamentoCriarDto, string idempotencyKey);
        Task<List<PagamentoResponseDto>> ListarPagamentoAsync();
        Task<PagamentoResponseDto> BuscarPagamentoIDAsync(int IDPagamento);
        Task<PagamentoDetalheDto> BuscarDetalhesAsync(int IDPagamento);
        Task<bool> CancelarPagamentoAsync(int IDPagamento);
        Task<bool> EstornarPagamentoAsync(int IDPagamento);
        Task<bool> ProcessarPagamentoAsync(int IDPagamento);
    }
}
