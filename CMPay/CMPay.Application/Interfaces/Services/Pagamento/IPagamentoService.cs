using CMPay.Application.DTOs;

namespace CMPay.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<int> CriarPagamentoAsync(PagamentoCriarDto pagamentoCriarDto);
        Task<List<PagamentoResponseDto>> ListarPagamentoAsync();
        Task<PagamentoResponseDto> BuscarPagamentoIDAsync(int IDPagamento);
        Task<PagamentoDetalheDto> BuscarDetalhesAsync(int IDPagamento);
    }
}
