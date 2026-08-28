using CMPay.Application.Interfaces;
using CMPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMPay.Infrastructure.Repositories.Pagamento
{
    public class PagamentoRepository : IPagamentoRepository
    {

        private readonly AppDbContext _context;

        public PagamentoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Domain.Entities.Pagamento pagamento)
        {
            await _context.Pagamentos.AddAsync(pagamento);
        }

        public async Task<Domain.Entities.Pagamento?> BuscarPorIDAsync(int IDPagamento)
        {
            return await _context.Pagamentos.FirstOrDefaultAsync(p => p.IDPagamento == IDPagamento); 
        }

        public Task<Domain.Entities.Pagamento?> BuscarPorIdempotencyKeyAsync(int idCliente, string idempotencyKey)
        {
            return _context.Pagamentos.FirstOrDefaultAsync(p => p.IDCliente == idCliente && p.IdempotencyKey == idempotencyKey);
        }

        public async Task<List<Domain.Entities.Pagamento>> BuscarTodosAsync()
        {
            return await _context.Pagamentos.ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
