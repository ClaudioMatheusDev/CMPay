using CMPay.Domain.Entities;

namespace CMPay.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> BuscarPorIDAsync(int IDCliente);
        Task<Cliente?> BuscarPorEmailAsync(string Email);
        Task<List<Cliente>> BuscarTodosClientesAsync();

        Task AdicionarClienteAsync(Cliente cliente);
        void Atualizar(Cliente cliente);
        void Remover(Cliente cliente);
        Task<Cliente?> BuscarPorApiKeyHashAsync(string hash);
        Task SalvarAlteracoesAsync();
    }
}
