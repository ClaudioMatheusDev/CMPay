using CMPay.Application.Interfaces;
using CMPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace CMPay.Infrastructure.Repositories.Cartao
{
    public class CartaoRepository : ICartaoRepository
    {

        private readonly AppDbContext _context;

        public CartaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarCartaoAsync(Domain.Entities.Cartao cartao)
        {
            await _context.Cartoes.AddAsync(cartao);
        }


        public async Task<Domain.Entities.Cartao?> BuscarCartaoPorCliente(int IDCliente)
        {
            return await _context.Cartoes.FirstOrDefaultAsync(c => c.IDCliente == IDCliente);
        }

        public async Task<Domain.Entities.Cartao?> BuscarCartaoPorIDAsync(int IDCartao)
        {
            return await _context.Cartoes.FirstOrDefaultAsync(c => c.IDCartao == IDCartao);
        }

        public async Task<List<Domain.Entities.Cartao>> BuscarTodosCartoes()
        {
            return await _context.Cartoes.ToListAsync();
        }

        public void Remover(Domain.Entities.Cartao cartao)
        {
            _context.Cartoes.Remove(cartao);
        }

        public void Atualizar(Domain.Entities.Cartao cartao)
        {
            _context.Cartoes.Update(cartao);
        }

        public async Task SalvarAlteracoesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
