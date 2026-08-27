using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Domain.Entities;
using CMPay.Domain.Enums;
using Moq;

namespace CMPay.Tests.Services
{
    public class PagamentoServiceTests
    {
        private static Cliente CriarCliente() => new()
        {
            IDCliente = 1,
            Nome = "Cliente Teste",
            Email = "cliente@teste.com",
            Documento = "12345678900",
            Telefone = "11999999999",
            DataCriacao = DateTime.UtcNow
        };

        private static Pagamento CriarPagamento(StatusPagamento status, decimal valorBruto = 100m) => new()
        {
            IDPagamento = 1,
            IDCliente = 1,
            ValorBruto = valorBruto,
            StatusPagamento = status,
            DataCriacao = DateTime.UtcNow
        };

        [Fact]
        public async Task CriarPagamentoAsync_MetodoPix_DeveAplicarTaxaDeUmPorCento()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var pagamentoRepositorio = new Mock<IPagamentoRepository>();
            pagamentoRepositorio
                .Setup(r => r.AdicionarAsync(It.IsAny<Pagamento>()))
                .Callback<Pagamento>(p => p.IDPagamento = 1)
                .Returns(Task.CompletedTask);

            var servico = new PagamentoService(
                pagamentoRepositorio.Object,
                clienteRepositorio.Object,
                Mock.Of<ITransacaoRepository>(),
                Mock.Of<IProcessadorPagamento>());

            var dto = new PagamentoCriarDto
            {
                IDCliente = 1,
                ValorBruto = 100m,
                Moeda = TipoMoeda.BRL,
                TipoMetodoPagamento = TipoMetodoPagamento.Pix
            };

            Pagamento? pagamentoCriado = null;
            pagamentoRepositorio
                .Setup(r => r.AdicionarAsync(It.IsAny<Pagamento>()))
                .Callback<Pagamento>(p => pagamentoCriado = p)
                .Returns(Task.CompletedTask);

            await servico.CriarPagamentoAsync(dto);

            Assert.NotNull(pagamentoCriado);
            Assert.Equal(1m, pagamentoCriado!.ValorTaxa);
            Assert.Equal(99m, pagamentoCriado.ValorLiquido);
        }

        [Fact]
        public async Task ProcessarPagamentoAsync_QuandoAprovado_PersisteAlteracoesViaRepositorioDePagamento()
        {
            var pagamento = CriarPagamento(StatusPagamento.Pendente);

            var pagamentoRepositorio = new Mock<IPagamentoRepository>();
            pagamentoRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(pagamento);

            var processador = new Mock<IProcessadorPagamento>();
            processador
                .Setup(p => p.ProcessarAsync(pagamento))
                .ReturnsAsync(new ProcessamentoPagamentoResultadoDto { Aprovado = true, Mensagem = "Pagamento aprovado." });

            var servico = new PagamentoService(
                pagamentoRepositorio.Object,
                Mock.Of<IClienteRepository>(),
                Mock.Of<ITransacaoRepository>(),
                processador.Object);

            await servico.ProcessarPagamentoAsync(1);

            Assert.Equal(StatusPagamento.Aprovado, pagamento.StatusPagamento);
            pagamentoRepositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task EstornarPagamentoAsync_PagamentoNaoAprovado_DeveLancarBusinessException()
        {
            var pagamento = CriarPagamento(StatusPagamento.Pendente);

            var pagamentoRepositorio = new Mock<IPagamentoRepository>();
            pagamentoRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(pagamento);

            var servico = new PagamentoService(
                pagamentoRepositorio.Object,
                Mock.Of<IClienteRepository>(),
                Mock.Of<ITransacaoRepository>(),
                Mock.Of<IProcessadorPagamento>());

            await Assert.ThrowsAsync<BusinessException>(() => servico.EstornarPagamentoAsync(1));
        }

        [Fact]
        public async Task BuscarPagamentoIDAsync_PagamentoInexistente_DeveLancarNotFoundException()
        {
            var pagamentoRepositorio = new Mock<IPagamentoRepository>();
            pagamentoRepositorio.Setup(r => r.BuscarPorIDAsync(It.IsAny<int>())).ReturnsAsync((Pagamento?)null);

            var servico = new PagamentoService(
                pagamentoRepositorio.Object,
                Mock.Of<IClienteRepository>(),
                Mock.Of<ITransacaoRepository>(),
                Mock.Of<IProcessadorPagamento>());

            await Assert.ThrowsAsync<NotFoundException>(() => servico.BuscarPagamentoIDAsync(1));
        }
    }
}
