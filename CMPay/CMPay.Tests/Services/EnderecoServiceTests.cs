using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Application.Services;
using CMPay.Domain.Entities;
using Moq;

namespace CMPay.Tests.Services
{
    public class EnderecoServiceTests
    {
        private static Endereco CriarEndereco(int id = 1, int idCliente = 1) => new()
        {
            IDEndereco = id,
            IDCliente = idCliente,
            Logradouro = "Rua Teste",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01000-000",
            Pais = "Brasil"
        };

        private static EnderecoCriarDto CriarEnderecoDto(int idCliente = 1) => new()
        {
            IDCliente = idCliente,
            Logradouro = "Rua Teste",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "São Paulo",
            Estado = "SP",
            Cep = "01000-000",
            Pais = "Brasil"
        };

        private static EnderecoAtualizarDto CriarEnderecoAtualizarDto(int idCliente = 1) => new()
        {
            IDCliente = idCliente,
            Logradouro = "Rua Atualizada",
            Numero = "200",
            Bairro = "Bairro Novo",
            Cidade = "Rio de Janeiro",
            Estado = "RJ",
            Cep = "20000-000",
            Pais = "Brasil"
        };

        [Fact]
        public async Task CriarEnderecoAsync_DadosValidos_DeveCriarComSucesso()
        {
            var repositorio = new Mock<IEnderecoRepository>();

            Endereco? enderecoCriado = null;
            repositorio
                .Setup(r => r.AdicionarEnderecoAsync(It.IsAny<Endereco>()))
                .Callback<Endereco>(e =>
                {
                    e.IDEndereco = 1;
                    enderecoCriado = e;
                })
                .Returns(Task.CompletedTask);

            var servico = new EnderecoService(repositorio.Object);

            var idEndereco = await servico.CriarEnderecoAsync(CriarEnderecoDto());

            Assert.Equal(1, idEndereco);
            Assert.NotNull(enderecoCriado);
            Assert.Equal("Rua Teste", enderecoCriado!.Logradouro);
            repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task BuscarEnderecoPorID_EnderecoInexistente_DeveLancarNotFoundException()
        {
            var repositorio = new Mock<IEnderecoRepository>();
            repositorio.Setup(r => r.BuscarEnderecoID(It.IsAny<int>())).ReturnsAsync((Endereco?)null);

            var servico = new EnderecoService(repositorio.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => servico.BuscarEnderecoPorID(1));
        }

        [Fact]
        public async Task BuscarEnderecoPorID_EnderecoExistente_DeveRetornarDto()
        {
            var repositorio = new Mock<IEnderecoRepository>();
            repositorio.Setup(r => r.BuscarEnderecoID(1)).ReturnsAsync(CriarEndereco());

            var servico = new EnderecoService(repositorio.Object);

            var resultado = await servico.BuscarEnderecoPorID(1);

            Assert.Equal("Rua Teste", resultado.Logradouro);
            Assert.Equal("SP", resultado.Estado);
        }

        [Fact]
        public async Task ApagarEnderecoAsync_EnderecoInexistente_DeveLancarNotFoundException()
        {
            var repositorio = new Mock<IEnderecoRepository>();
            repositorio.Setup(r => r.BuscarEnderecoID(It.IsAny<int>())).ReturnsAsync((Endereco?)null);

            var servico = new EnderecoService(repositorio.Object);

            await Assert.ThrowsAsync<NotFoundException>(() => servico.ApagarEnderecoAsync(1));
        }

        [Fact]
        public async Task ApagarEnderecoAsync_EnderecoExistente_DeveRemoverEndereco()
        {
            var endereco = CriarEndereco();

            var repositorio = new Mock<IEnderecoRepository>();
            repositorio.Setup(r => r.BuscarEnderecoID(1)).ReturnsAsync(endereco);

            var servico = new EnderecoService(repositorio.Object);

            await servico.ApagarEnderecoAsync(1);

            repositorio.Verify(r => r.Remover(endereco), Times.Once);
            repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }

        [Fact]
        public async Task AtualizarEnderecoAsync_EnderecoInexistente_DeveLancarNotFoundException()
        {
            var repositorio = new Mock<IEnderecoRepository>();
            repositorio.Setup(r => r.BuscarEnderecoID(It.IsAny<int>())).ReturnsAsync((Endereco?)null);

            var servico = new EnderecoService(repositorio.Object);

            await Assert.ThrowsAsync<NotFoundException>(
                () => servico.AtualizarEnderecoAsync(1, CriarEnderecoAtualizarDto()));
        }

        [Fact]
        public async Task AtualizarEnderecoAsync_DadosValidos_DeveAtualizarComSucesso()
        {
            var endereco = CriarEndereco();

            var repositorio = new Mock<IEnderecoRepository>();
            repositorio.Setup(r => r.BuscarEnderecoID(1)).ReturnsAsync(endereco);

            var servico = new EnderecoService(repositorio.Object);

            var resultado = await servico.AtualizarEnderecoAsync(1, CriarEnderecoAtualizarDto());

            Assert.Equal("Rua Atualizada", resultado.Logradouro);
            Assert.Equal("RJ", resultado.Estado);
            repositorio.Verify(r => r.SalvarAlteracoesAsync(), Times.Once);
        }
    }
}
