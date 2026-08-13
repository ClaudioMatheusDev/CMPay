using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;
using CMPay.Domain.Enums.Cartao;

namespace CMPay.Application.Services
{
    public class CartaoService : ICartaoService
    {
        private readonly ICartaoRepository _cartaoRepository;
        private readonly IClienteRepository _clienteRepository;

        public CartaoService(ICartaoRepository cartaoRepository, IClienteRepository clienteRepository)
        {
            _cartaoRepository = cartaoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<int> CriarCartaoAsync(CartaoCriarDto cartaoCriarDto)
        {
            var clienteExiste = await _clienteRepository.BuscarPorIDAsync(cartaoCriarDto.IDCliente);

            if (clienteExiste == null)
            {
                throw new Exception("Cliente não existe");
            }

            var cartao = new Cartao
            {
                IDCliente = cartaoCriarDto.IDCliente,
                BandeiraCartao = cartaoCriarDto.BandeiraCartao,
                UltimosDigitos = cartaoCriarDto.UltimosDigitos,
                MesExpiracao = cartaoCriarDto.MesExpiracao,
                AnoExpiracao = cartaoCriarDto.AnoExpiracao,
                NomeTitular = cartaoCriarDto.NomeTitular,
                Padrao = cartaoCriarDto.Padrao
            };

            await _cartaoRepository.AdicionarCartaoAsync(cartao);

            await _cartaoRepository.SalvarAlteracoesAsync();

            return cartao.IDCartao;
        }


        public async Task<CartaoResponseDto> BuscarCartaoPorIDAsync(int IDCartao)
        {
            var cartao = await _cartaoRepository.BuscarCartaoPorIDAsync(IDCartao);

            if (cartao == null)
            {
                throw new Exception("Nenhum cartao encontrado com esse IDCartao.");
            }


            return new CartaoResponseDto
            {
                IDCartao = cartao.IDCartao,
                IDCliente = cartao.IDCliente,
                BandeiraCartao = cartao.BandeiraCartao,
                UltimosDigitos = cartao.UltimosDigitos,
                MesExpiracao = cartao.MesExpiracao,
                AnoExpiracao = cartao.AnoExpiracao,
                NomeTitular = cartao.NomeTitular,
                Padrao = cartao.Padrao,
                Ativo = cartao.Ativo,
                DataCriacao = cartao.DataCriacao
            };

        }

        public async Task<List<CartaoResponseDto>> BuscarTodosAsync()
        {
            var cartao = await _cartaoRepository.BuscarTodosCartoes();

            return cartao.Select(cartao => new CartaoResponseDto
            {
                IDCartao = cartao.IDCartao,
                IDCliente = cartao.IDCliente,
                BandeiraCartao = cartao.BandeiraCartao,
                UltimosDigitos = cartao.UltimosDigitos,
                MesExpiracao = cartao.MesExpiracao,
                AnoExpiracao = cartao.AnoExpiracao,
                NomeTitular = cartao.NomeTitular,
                Padrao = cartao.Padrao,
                Ativo = cartao.Ativo,
                DataCriacao = cartao.DataCriacao
            }).ToList();
        }

        public async Task<CartaoResponseDto> AtualizarClienteAsync(int IDCartao, CartaoAtualizarDto cartaoAtualizarDto)
        {
            var cartao = await _cartaoRepository.BuscarCartaoPorIDAsync(IDCartao);
            var cliente = await _clienteRepository.BuscarPorIDAsync(cartaoAtualizarDto.IDCliente);

            if (cliente == null)
            {
                throw new Exception("Nenhum cliente encontrado com esse IDCliente.");
            }

            cartao.IDCliente = cartaoAtualizarDto.IDCliente;
            cartao.BandeiraCartao = cartaoAtualizarDto.BandeiraCartao;
            cartao.UltimosDigitos = cartaoAtualizarDto.UltimosDigitos;
            cartao.MesExpiracao = cartaoAtualizarDto.MesExpiracao;
            cartao.AnoExpiracao = cartaoAtualizarDto.AnoExpiracao;
            cartao.NomeTitular = cartaoAtualizarDto.NomeTitular;
            cartao.Padrao = cartaoAtualizarDto.Padrao;
            cartao.Ativo = cartaoAtualizarDto.Ativo;

            await _cartaoRepository.SalvarAlteracoesAsync();

            return new CartaoResponseDto
            {
                IDCliente = cartao.IDCliente,
                BandeiraCartao = cartao.BandeiraCartao,
                UltimosDigitos = cartao.UltimosDigitos,
                MesExpiracao = cartao.MesExpiracao,
                AnoExpiracao = cartao.AnoExpiracao,
                NomeTitular = cartao.NomeTitular,
                Padrao = cartao.Padrao,
                Ativo = cartao.Ativo
            };

        }

        public Task<bool> ApagarCartaoAsync(int IDCartao)
        {
            throw new NotImplementedException();
        }

    }
}
