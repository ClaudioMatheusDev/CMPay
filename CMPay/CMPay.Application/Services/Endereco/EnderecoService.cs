using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;

namespace CMPay.Application.Services
{
    public class EnderecoService : IEnderecoService
    {

        private readonly IEnderecoRepository _enderecoRepository;

        public EnderecoService(IEnderecoRepository enderecoRepository)
        {
            _enderecoRepository = enderecoRepository;
        }

        public async Task<int> CriarEnderecoAsync(EnderecoCriarDto enderecoCriarDto)
        {
            var endereco = new Endereco
            {
                IDCliente = enderecoCriarDto.IDCliente,
                Logradouro = enderecoCriarDto.Logradouro,
                Numero = enderecoCriarDto.Numero,
                Complemento = enderecoCriarDto.Complemento,
                Bairro = enderecoCriarDto.Bairro,
                Cidade = enderecoCriarDto.Cidade,
                Estado = enderecoCriarDto.Estado,
                Cep = enderecoCriarDto.Cep,
                Pais = enderecoCriarDto.Pais
            };

            await _enderecoRepository.AdicionarEnderecoAsync(endereco);

            await _enderecoRepository.SalvarAlteracoesAsync();

            return endereco.IDEndereco;
        }

        public async Task<EnderecoResponseDto> BuscarEnderecoPorID(int IDEndereco)
        {
            var endereco = await _enderecoRepository.BuscarEnderecoID(IDEndereco);

            if (endereco == null)
            {
                throw new NotFoundException("Nenhum endereco encontrado com esse IDEndereco.");
            }

            return new EnderecoResponseDto
            {
                IDEndereco = endereco.IDEndereco,
                IDCliente = endereco.IDCliente,
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep,
                Pais = endereco.Pais
            };
        }

        public async Task<List<EnderecoResponseDto>> BuscarTodosEndereco()
        {
            var endereco = await _enderecoRepository.BuscarTodosEnderecos();

            return endereco.Select(endereco => new EnderecoResponseDto
            {
                IDEndereco = endereco.IDEndereco,
                IDCliente = endereco.IDCliente,
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep,
                Pais = endereco.Pais
            }).ToList();
        }


        public async Task<bool> ApagarEnderecoAsync(int IDEndereco)
        {
            var endereco = await _enderecoRepository.BuscarEnderecoID(IDEndereco);

            if (endereco == null)
            {
                throw new NotFoundException("Nenhum endereco encontrado com esse IDEndereco.");
            }

            _enderecoRepository.Remover(endereco);

            await _enderecoRepository.SalvarAlteracoesAsync();

            return true;
        }

        public async Task<EnderecoResponseDto> AtualizarEnderecoAsync(int IDEndereco, EnderecoAtualizarDto enderecoAtualizarDto)
        {
            var endereco = await _enderecoRepository.BuscarEnderecoID(IDEndereco);

            if (endereco == null)
            {
                throw new NotFoundException("Nenhum endereco encontrado com esse IDEndereco.");
            }

            endereco.IDCliente = enderecoAtualizarDto.IDCliente;
            endereco.Logradouro = enderecoAtualizarDto.Logradouro;
            endereco.Numero = enderecoAtualizarDto.Numero;
            endereco.Complemento = enderecoAtualizarDto.Complemento;
            endereco.Bairro = enderecoAtualizarDto.Bairro;
            endereco.Cidade = enderecoAtualizarDto.Cidade;
            endereco.Estado = enderecoAtualizarDto.Estado;
            endereco.Cep = enderecoAtualizarDto.Cep;
            endereco.Pais = enderecoAtualizarDto.Pais;

            await _enderecoRepository.SalvarAlteracoesAsync();

            return new EnderecoResponseDto
            {
                IDEndereco = endereco.IDEndereco,
                IDCliente = endereco.IDCliente,
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep,
                Pais = endereco.Pais
            };

        }
    }
}
