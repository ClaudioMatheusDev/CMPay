using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Domain.Entities;
using Moq;

namespace CMPay.Tests.Services
{
    public class ClienteServiceTests
    {
        private static Cliente CriarCliente(int id, string email) => new()
        {
            IDCliente = id,
            Nome = "Cliente Teste",
            Email = email,
            Documento = "12345678900",
            Telefone = "11999999999",
            DataCriacao = DateTime.UtcNow
        };

        private static ClienteAtualizarDto CriarDto(string email) => new()
        {
            Nome = "Cliente Teste",
            Email = email,
            Documento = "12345678900",
            Telefone = "11999999999",
            DataNascimento = new DateTime(1990, 1, 1)
        };

        [Fact]
        public async Task AtualizarClienteAsync_MantendoMesmoEmail_NaoDeveLancarExcecao()
        {
            var cliente = CriarCliente(1, "cliente@teste.com");

            var repositorio = new Mock<IClienteRepository>();
            repositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(cliente);
            repositorio.Setup(r => r.BuscarPorEmailAsync("cliente@teste.com")).ReturnsAsync(cliente);

            var servico = new ClienteService(repositorio.Object);

            var resultado = await servico.AtualizarClienteAsync(1, CriarDto("cliente@teste.com"));

            Assert.Equal("cliente@teste.com", resultado.Email);
        }

        [Fact]
        public async Task AtualizarClienteAsync_EmailJaUsadoPorOutroCliente_DeveLancarBusinessException()
        {
            var cliente = CriarCliente(1, "cliente@teste.com");
            var outroCliente = CriarCliente(2, "outro@teste.com");

            var repositorio = new Mock<IClienteRepository>();
            repositorio.Setup(r => r.BuscarPorIDAsync(1)).ReturnsAsync(cliente);
            repositorio.Setup(r => r.BuscarPorEmailAsync("outro@teste.com")).ReturnsAsync(outroCliente);

            var servico = new ClienteService(repositorio.Object);

            await Assert.ThrowsAsync<BusinessException>(
                () => servico.AtualizarClienteAsync(1, CriarDto("outro@teste.com")));
        }

        [Fact]
        public async Task ApagarClienteAsync_ClienteInexistente_DeveLancarNotFoundException()
        {
            var repositorio = new Mock<IClienteRepository>();
            repositorio.Setup(r => r.BuscarPorIDAsync(It.IsAny<int>())).ReturnsAsync((Cliente?)null);

            var servico = new ClienteService(repositorio.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => servico.ApagarClienteAsync(1));
        }
    }
}
