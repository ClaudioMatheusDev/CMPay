using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using CMPay.Applicatios.Interfaces;
using CMPay.Domain.Entities;
using System.Data;

namespace CMPay.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<int> CriarClienteAsync(ClienteCriarDto clienteCriarDto)
        {
            var ClienteExiste = await _clienteRepository.BuscarPorEmailAsync(clienteCriarDto.Email);

            if (ClienteExiste != null)
            {
                throw new Exception("Já existe um cliente com esse E-mail.");
            }

            var cliente = new Cliente
            {
                Nome = clienteCriarDto.Nome,
                DataNascimento = clienteCriarDto.DataNascimento,
                Email = clienteCriarDto.Email,
                Documento = clienteCriarDto.Documento,
                Telefone = clienteCriarDto.Telefone,
                DataCriacao = DateTime.UtcNow.AddHours(-3)
            };

            await _clienteRepository.AdicionarClienteAsync(cliente);

            await _clienteRepository.SalvarAlteracoesAsync();

            return cliente.IDCliente;
        }

        public async Task<ClienteResponseDto?> BuscarClientePorIDAsync(int IDCliente)
        {
            var cliente = await _clienteRepository.BuscarPorIDAsync(IDCliente);


            if (cliente == null)
            {
                throw new Exception("Nenhum cliente encontrado com esse IDCliente.");
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
                throw new Exception("Nenhum cliente encontrado com esse IDCliente.");
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
                throw new Exception("Nenhum cliente encontrado com esse IDCliente.");
            }

            var ClienteExiste =
            await _clienteRepository.BuscarPorEmailAsync(clienteAtualizarDto.Email);

            if (ClienteExiste != null)
            {
                throw new Exception("Já existe um cliente com esse E-mail.");
            }

            cliente.Nome = clienteAtualizarDto.Nome;
            cliente.Telefone = clienteAtualizarDto.Telefone;
            cliente.Documento = clienteAtualizarDto.Documento;
            cliente.Email = clienteAtualizarDto.Email;
            cliente.DataNascimento = clienteAtualizarDto.DataNascimento;
            cliente.DataAtualizacao = DateTime.UtcNow.AddHours(-3);

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
