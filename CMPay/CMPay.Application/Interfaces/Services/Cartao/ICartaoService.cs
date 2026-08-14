using CMPay.Application.DTOs;

namespace CMPay.Application.Interfaces
{
    public interface ICartaoService
    {
        Task<int> CriarCartaoAsync(CartaoCriarDto cartaoCriarDto);
        Task<CartaoResponseDto> BuscarCartaoPorIDAsync(int IDCartao);
        Task<List<CartaoResponseDto>> BuscarTodosAsync();
        Task<bool> ApagarCartaoAsync(int IDCartao);
        Task<CartaoResponseDto> AtualizarCartaoAsync(int IDCartao, CartaoAtualizarDto cartaoAtualizarDto);
    }
}
