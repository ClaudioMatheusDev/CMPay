using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;
using CMPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CMPay.Infrastructure.Repositories
{
    public class TransacaoRepository : ITransacaoRepository
    {

        private readonly AppDbContext _context;

        public TransacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Transacao transacao)
        {
            await _context.Transacoes.AddAsync(transacao);
        }

        public async Task<List<Transacao>> BuscarPorPagamentoAsync(int IDPagamento)
        {
            return await _context.Transacoes.Where(t => t.IDPagamento == IDPagamento).OrderBy(t => t.DataTransacao).ToListAsync();
        }

        public async Task SalvarAlteracoesAsync()
        {
           await _context.SaveChangesAsync();
        }


    }
}
