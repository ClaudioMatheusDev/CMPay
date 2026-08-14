using CMPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMPay.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cartao> Cartoes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Endereco)
                .WithOne(e => e.Cliente)
                .HasForeignKey<Endereco>(e => e.IDCliente) 
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cartao>()
                .HasOne(ca => ca.Cliente)
                .WithMany(c => c.Cartoes)
                .HasForeignKey(ca => ca.IDCliente)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pagamento>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pagamentos)
                .HasForeignKey(p => p.IDCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.Pagamento)
                .WithMany(p => p.Transacoes)
                .HasForeignKey(t => t.IDPagamento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pagamento>()
                .Property(p => p.ValorBruto)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pagamento>()
                .Property(p => p.ValorTaxa)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Pagamento>()
                .Property(p => p.ValorLiquido)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transacao>()
                .Property(t => t.Valor)
                .HasPrecision(18, 2);
        }
    }
}
