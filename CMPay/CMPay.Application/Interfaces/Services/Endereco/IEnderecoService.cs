using CMPay.Application.DTOs;

namespace CMPay.Application.Interfaces
{
    public interface IEnderecoService
    {
        Task<int> CriarEnderecoAsync(EnderecoCriarDto enderecoCriarDto);
        Task<EnderecoResponseDto> BuscarEnderecoPorID(int IDEndereco);
        Task<List<EnderecoResponseDto>> BuscarTodosEndereco();
        Task<bool> ApagarEnderecoAsync(int IDEndereco);
        Task<EnderecoResponseDto> AtualizarEnderecoAsync(int IDEndereco, EnderecoAtualizarDto enderecoAtualizarDto);
    }
}
