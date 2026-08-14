using CMPay.Domain.Entities;

namespace CMPay.Application.Interfaces
{
    public interface IEnderecoRepository
    {
        Task<Endereco?> BuscarEnderecoID(int IDEndereco);
        Task<Endereco?> BuscarEnderecoPorIDCliente(int IDCliente);

        Task<List<Endereco>> BuscarTodosEnderecos();

        Task AdicionarEnderecoAsync(Endereco endereco);
        void Atualizar(Endereco endereco);
        void Remover(Endereco endereco);
        Task SalvarAlteracoesAsync();
    }
}
