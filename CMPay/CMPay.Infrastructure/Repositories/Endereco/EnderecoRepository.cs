using CMPay.Application.Interfaces;
using CMPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMPay.Infrastructure.Repositories.Endereco
{
    public class EnderecoRepository : IEnderecoRepository
    {
        private readonly AppDbContext _context;

        public EnderecoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarEnderecoAsync(Domain.Entities.Endereco endereco)
        {
            await _context.Enderecos.AddAsync(endereco);
        }

        public async Task<Domain.Entities.Endereco?> BuscarEnderecoID(int IDEndereco)
        {
            return await _context.Enderecos.FirstOrDefaultAsync(e => e.IDEndereco == IDEndereco);
        }

        public async Task<Domain.Entities.Endereco?> BuscarEnderecoPorIDCliente(int IDCliente)
        {
            return await _context.Enderecos.FirstOrDefaultAsync(e => e.IDCliente == IDCliente);
        }

        public async Task<List<Domain.Entities.Endereco>> BuscarTodosEnderecos()
        {
            return await _context.Enderecos.ToListAsync();
        }

        public void Remover(Domain.Entities.Endereco endereco)
        {
            _context.Remove(endereco);
        }
        public void Atualizar(Domain.Entities.Endereco endereco)
        {
            _context.Enderecos.Update(endereco);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
