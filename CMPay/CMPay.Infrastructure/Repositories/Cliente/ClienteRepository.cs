using CMPay.Application.Interfaces;
using CMPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMPay.Infrastructure.Repositories.Cliente
{
    public class ClienteRepository : IClienteRepository
    {

        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Domain.Entities.Cliente?> BuscarPorIDAsync(int IDCliente)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.IDCliente == IDCliente);
        }

        public async Task<Domain.Entities.Cliente?> BuscarPorEmailAsync(string Email)
        {
            return await _context.Clientes.FirstOrDefaultAsync(e => e.Email == Email);
        }

        public async Task<List<Domain.Entities.Cliente>> BuscarTodosClientesAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task AdicionarClienteAsync(Domain.Entities.Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
        }

        public void Atualizar(Domain.Entities.Cliente cliente)
        {
            _context.Clientes.Update(cliente);
        }

        public void Remover(Domain.Entities.Cliente cliente)
        {
             _context.Remove(cliente);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
