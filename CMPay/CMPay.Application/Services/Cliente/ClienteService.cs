using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace CMPay.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<ClienteCriadoDto> CriarClienteAsync(ClienteCriarDto clienteCriarDto)
        {
            var ClienteExiste = await _clienteRepository.BuscarPorEmailAsync(clienteCriarDto.Email);



            if (ClienteExiste != null)
            {
                throw new BusinessException("Já existe um cliente com esse E-mail.");
            }

            var apiKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var apiKeyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(apiKey)));

            var cliente = new Cliente
            {
                Nome = clienteCriarDto.Nome,
                DataNascimento = clienteCriarDto.DataNascimento,
                Email = clienteCriarDto.Email,
                Documento = clienteCriarDto.Documento,
                Telefone = clienteCriarDto.Telefone,
                DataCriacao = DateTime.UtcNow,
                ApiKeyHash = apiKeyHash
            };

            await _clienteRepository.AdicionarClienteAsync(cliente);

            await _clienteRepository.SalvarAlteracoesAsync();

            return new ClienteCriadoDto
            {
                IDCliente = cliente.IDCliente,
                ApiKey = apiKey
            };
        }

        public async Task<ClienteResponseDto?> BuscarClientePorIDAsync(int IDCliente)
        {
            var cliente = await _clienteRepository.BuscarPorIDAsync(IDCliente);


            if (cliente == null)
            {
                throw new NotFoundException("Nenhum cliente encontrado com esse IDCliente.");
            }

            return new ClienteResponseDto
            {
                IDCliente = cliente.IDCliente,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Documento = cliente.Documento,
                DataCriacao = cliente.DataCriacao
            };
        }

        public async Task<List<ClienteResponseDto>> BuscarTodosAsync()
        {
            var clientes = await _clienteRepository.BuscarTodosClientesAsync();

            return clientes.Select(cliente => new ClienteResponseDto
            {
                IDCliente = cliente.IDCliente,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Documento = cliente.Documento,
                DataCriacao = cliente.DataCriacao
            }).ToList();
        }

        public async Task<bool> ApagarClienteAsync(int IDCliente)
        {
            var cliente = await _clienteRepository.BuscarPorIDAsync(IDCliente);

            if (cliente == null)
            {
                throw new NotFoundException("Nenhum cliente encontrado com esse IDCliente.");
            }

            _clienteRepository.Remover(cliente);

            await _clienteRepository.SalvarAlteracoesAsync();

            return true;
        }

        public async Task<ClienteResponseDto> AtualizarClienteAsync(int IDCliente, ClienteAtualizarDto clienteAtualizarDto)
        {
            var cliente = await _clienteRepository.BuscarPorIDAsync(IDCliente);

            if (cliente == null)
            {
                throw new NotFoundException("Nenhum cliente encontrado com esse IDCliente.");
            }

            var ClienteExiste =
            await _clienteRepository.BuscarPorEmailAsync(clienteAtualizarDto.Email);

            if (ClienteExiste != null && ClienteExiste.IDCliente != IDCliente)
            {
                throw new BusinessException("Já existe um cliente com esse E-mail.");
            }

            cliente.Nome = clienteAtualizarDto.Nome;
            cliente.Telefone = clienteAtualizarDto.Telefone;
            cliente.Documento = clienteAtualizarDto.Documento;
            cliente.Email = clienteAtualizarDto.Email;
            cliente.DataNascimento = clienteAtualizarDto.DataNascimento;
            cliente.DataAtualizacao = DateTime.UtcNow;

            await _clienteRepository.SalvarAlteracoesAsync();

            return new ClienteResponseDto
            {
                IDCliente = cliente.IDCliente,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Documento = cliente.Documento,
                DataCriacao = cliente.DataCriacao
            };

        }
    }
}
