using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Domain.Entities;
using CMPay.Domain.Enums;
using Moq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CMPay.Tests.Services
{
    // Replica o cálculo de hash privado de PagamentoService para montar cenários de
    // teste (payload igual/diferente) sem expor o método de produção.
    internal static class PagamentoServiceTestsHashHelper
    {
        public static string CalcularHash(PagamentoCriarDto dto)
        {
            var payloadCanonico = JsonSerializer.Serialize(new
            {
                dto.IDCliente,
                dto.ValorBruto,
                dto.Moeda,
                dto.TipoMetodoPagamento
            });

            var bytesHash = SHA256.HashData(Encoding.UTF8.GetBytes(payloadCanonico));
            return Convert.ToHexString(bytesHash);
        }
    }

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
                .Setup(r => r.BuscarPorIdempotencyKeyAsync(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync((Pagamento?)null);
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

            await servico.CriarPagamentoAsync(dto, "chave-idempotencia-1");

            Assert.NotNull(pagamentoCriado);
            Assert.Equal(1m, pagamentoCriado!.ValorTaxa);
            Assert.Equal(99m, pagamentoCriado.ValorLiquido);
        }

        [Fact]
        public async Task CriarPagamentoAsync_MesmaChaveMesmoPayload_DeveRetornarPagamentoExistenteSemCriarNovo()
        {
            var dto = new PagamentoCriarDto
            {
                IDCliente = 1,
                ValorBruto = 100m,
                Moeda = TipoMoeda.BRL,
                TipoMetodoPagamento = TipoMetodoPagamento.Pix
            };

            var pagamentoExistente = new Pagamento
            {
                IDPagamento = 42,
                IDCliente = 1,
                IdempotencyKey = "chave-repetida",
                PayloadHash = PagamentoServiceTestsHashHelper.CalcularHash(dto)
            };

            var pagamentoRepositorio = new Mock<IPagamentoRepository>();
            pagamentoRepositorio
                .Setup(r => r.BuscarPorIdempotencyKeyAsync(1, "chave-repetida"))
                .ReturnsAsync(pagamentoExistente);

            var servico = new PagamentoService(
                pagamentoRepositorio.Object,
                Mock.Of<IClienteRepository>(),
                Mock.Of<ITransacaoRepository>(),
                Mock.Of<IProcessadorPagamento>());

            var idPagamento = await servico.CriarPagamentoAsync(dto, "chave-repetida");

            Assert.Equal(42, idPagamento);
            pagamentoRepositorio.Verify(r => r.AdicionarAsync(It.IsAny<Pagamento>()), Times.Never);
        }

        [Fact]
        public async Task CriarPagamentoAsync_MesmaChavePayloadDiferente_DeveLancarConflictException()
        {
            var dtoOriginal = new PagamentoCriarDto
            {
                IDCliente = 1,
                ValorBruto = 100m,
                Moeda = TipoMoeda.BRL,
                TipoMetodoPagamento = TipoMetodoPagamento.Pix
            };

            var dtoConflitante = new PagamentoCriarDto
            {
                IDCliente = 1,
                ValorBruto = 200m,
                Moeda = TipoMoeda.BRL,
                TipoMetodoPagamento = TipoMetodoPagamento.Pix
            };

            var pagamentoExistente = new Pagamento
            {
                IDPagamento = 42,
                IDCliente = 1,
                IdempotencyKey = "chave-repetida",
                PayloadHash = PagamentoServiceTestsHashHelper.CalcularHash(dtoOriginal)
            };

            var pagamentoRepositorio = new Mock<IPagamentoRepository>();
            pagamentoRepositorio
                .Setup(r => r.BuscarPorIdempotencyKeyAsync(1, "chave-repetida"))
                .ReturnsAsync(pagamentoExistente);

            var servico = new PagamentoService(
                pagamentoRepositorio.Object,
                Mock.Of<IClienteRepository>(),
                Mock.Of<ITransacaoRepository>(),
                Mock.Of<IProcessadorPagamento>());

            await Assert.ThrowsAsync<ConflictException>(
                () => servico.CriarPagamentoAsync(dtoConflitante, "chave-repetida"));
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
