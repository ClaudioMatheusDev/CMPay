using CMPay.Application.DTOs;

namespace CMPay.Application.Interfaces
{
    public interface IClienteService
    {
        Task<int> CriarClienteAsync(ClienteCriarDto clienteCriarDto);
        Task<ClienteResponseDto> BuscarClientePorIDAsync(int IDCliente);
        Task<List<ClienteResponseDto>> BuscarTodosAsync();
        Task<bool> ApagarClienteAsync(int IDCliente);
        Task<ClienteResponseDto> AtualizarClienteAsync(int IDCliente, ClienteAtualizarDto clienteAtualizarDto);
    }
}
