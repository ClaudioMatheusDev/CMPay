using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Domain.Entities;
using CMPay.Domain.Enums.Cartao;
using Moq;

namespace CMPay.Tests.Services
{
    public class CartaoServiceTests
    {
        private static Cliente CriarCliente(int id = 1) => new()
        {
            IDCliente = id,
            Nome = "Cliente Teste",
            Email = "cliente@teste.com",
            Documento = "12345678900",
            Telefone = "11999999999",
            DataCriacao = DateTime.UtcNow
        };

        private static Cartao CriarCartao(int id, int idCliente, bool padrao) => new()
        {
            IDCartao = id,
            IDCliente = idCliente,
            BandeiraCartao = BandeiraCartao.Visa,
            UltimosDigitos = "1234",
            NomeTitular = "Cliente Teste",
            Padrao = padrao,
            Ativo = true
        };

        private static CartaoCriarDto CriarCartaoDto(int idCliente = 1, bool padrao = false)
        {
            var expiracao = DateTime.UtcNow.AddYears(1);

            return new CartaoCriarDto
            {
                IDCliente = idCliente,
                BandeiraCartao = BandeiraCartao.Visa,
                UltimosDigitos = "1234",
                MesExpiracao = expiracao.Month,
                AnoExpiracao = expiracao.Year,
                NomeTitular = "Cliente Teste",
                Padrao = padrao
            };
        }

        [Fact]
        public async Task CriarCartaoAsync_DadosValidos_DeveCriarComSucesso()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorCliente(1)).ReturnsAsync((Cartao?)null);

            Cartao? cartaoCriado = null;
            cartaoRepositorio
                .Setup(r => r.AdicionarCartaoAsync(It.IsAny<Cartao>()))
                .Callback<Cartao>(c =>
                {
                    c.IDCartao = 10;
                    cartaoCriado = c;
                })
                .Returns(Task.CompletedTask);

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            var idCartao = await servico.CriarCartaoAsync(CriarCartaoDto());

            Assert.Equal(10, idCartao);
            Assert.NotNull(cartaoCriado);
            Assert.True(cartaoCriado!.Ativo);
            cartaoRepositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task CriarCartaoAsync_ClienteInexistente_DeveLancarNotFoundException()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

            var cartaoRepositorio = new Mock<ICartaoRepository>();

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => servico.CriarCartaoAsync(CriarCartaoDto()));
        }

        [Fact]
        public async Task CriarCartaoAsync_MesExpiracaoInvalido_DeveLancarBusinessException()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var cartaoRepositorio = new Mock<ICartaoRepository>();

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            var dto = CriarCartaoDto();
            dto.MesExpiracao = 13;

            await Assert.ThrowsAsync<BusinessException>(() => servico.CriarCartaoAsync(dto));
        }

        [Fact]
        public async Task CriarCartaoAsync_UltimosDigitosInvalidos_DeveLancarBusinessException()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var cartaoRepositorio = new Mock<ICartaoRepository>();

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            var dto = CriarCartaoDto();
            dto.UltimosDigitos = "12a4";

            await Assert.ThrowsAsync<BusinessException>(() => servico.CriarCartaoAsync(dto));
        }

        [Fact]
        public async Task CriarCartaoAsync_CartaoExpirado_DeveLancarBusinessException()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var cartaoRepositorio = new Mock<ICartaoRepository>();

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            var passada = DateTime.UtcNow.AddYears(-1);
            var dto = CriarCartaoDto();
            dto.MesExpiracao = passada.Month;
            dto.AnoExpiracao = passada.Year;

            await Assert.ThrowsAsync<BusinessException>(() => servico.CriarCartaoAsync(dto));
        }

        [Fact]
        public async Task CriarCartaoAsync_JaExisteCartaoPadrao_DeveLancarBusinessException()
        {
            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio
                .Setup(r => r.BuscarCartaoPorCliente(1))
                .ReturnsAsync(CriarCartao(id: 99, idCliente: 1, padrao: true));

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            var dto = CriarCartaoDto(padrao: true);

            await Assert.ThrowsAsync<BusinessException>(() => servico.CriarCartaoAsync(dto));
        }

        [Fact]
        public async Task BuscarCartaoPorIDAsync_CartaoInexistente_DeveLancarNotFoundException()
        {
            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorIDAsync(It.IsAny<int>())).ReturnsAsync((Cartao?)null);

            var servico = new CartaoService(cartaoRepositorio.Object, Mock.Of<IClienteRepository>());

            await Assert.ThrowsAsync<NotFoundException>(() => servico.BuscarCartaoPorIDAsync(1));
        }

        [Fact]
        public async Task AtualizarCartaoAsync_CartaoInexistente_DeveLancarNotFoundException()
        {
            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorIDAsync(It.IsAny<int>())).ReturnsAsync((Cartao?)null);

            var servico = new CartaoService(cartaoRepositorio.Object, Mock.Of<IClienteRepository>());

            var dto = CriarCartaoAtualizarDto();

            await Assert.ThrowsAsync<NotFoundException>(() => servico.AtualizarCartaoAsync(1, dto));
        }

        [Fact]
        public async Task AtualizarCartaoAsync_MesmoCartaoJaEhPadrao_NaoDeveLancarExcecao()
        {
            var cartaoExistente = CriarCartao(id: 5, idCliente: 1, padrao: true);

            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorIDAsync(5)).ReturnsAsync(cartaoExistente);
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorCliente(1)).ReturnsAsync(cartaoExistente);

            var clienteRepositorio = new Mock<IClienteRepository>();
            clienteRepositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(CriarCliente());

            var servico = new CartaoService(cartaoRepositorio.Object, clienteRepositorio.Object);

            var dto = CriarCartaoAtualizarDto(padrao: true);

            var resultado = await servico.AtualizarCartaoAsync(5, dto);

            Assert.Equal("4321", resultado.UltimosDigitos);
            Assert.True(resultado.Padrao);
        }

        [Fact]
        public async Task ApagarCartaoAsync_CartaoInexistente_DeveLancarNotFoundException()
        {
            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorIDAsync(It.IsAny<int>())).ReturnsAsync((Cartao?)null);

            var servico = new CartaoService(cartaoRepositorio.Object, Mock.Of<IClienteRepository>());

            await Assert.ThrowsAsync<NotFoundException>(() => servico.ApagarCartaoAsync(1));
        }

        [Fact]
        public async Task ApagarCartaoAsync_CartaoExistente_DeveDesativarCartao()
        {
            var cartao = CriarCartao(id: 1, idCliente: 1, padrao: true);

            var cartaoRepositorio = new Mock<ICartaoRepository>();
            cartaoRepositorio.Setup(r => r.BuscarCartaoPorIDAsync(1)).ReturnsAsync(cartao);

            var servico = new CartaoService(cartaoRepositorio.Object, Mock.Of<IClienteRepository>());

            await servico.ApagarCartaoAsync(1);

            Assert.False(cartao.Ativo);
            Assert.False(cartao.Padrao);
            cartaoRepositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        private static CartaoAtualizarDto CriarCartaoAtualizarDto(bool padrao = false)
        {
            var expiracao = DateTime.UtcNow.AddYears(1);

            return new CartaoAtualizarDto
            {
                IDCliente = 1,
                BandeiraCartao = BandeiraCartao.MasterCard,
                UltimosDigitos = "4321",
                MesExpiracao = expiracao.Month,
                AnoExpiracao = expiracao.Year,
                NomeTitular = "Cliente Teste Atualizado",
                Padrao = padrao,
                Ativo = true
            };
        }
    }
}
