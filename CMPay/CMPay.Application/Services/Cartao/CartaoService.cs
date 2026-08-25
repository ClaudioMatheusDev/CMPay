using CMPay.Application.DTOs;
using CMPay.Application.Exceptions;
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
            await ValidarCartaoAsync(
                cartaoCriarDto.IDCliente,
                cartaoCriarDto.MesExpiracao,
                cartaoCriarDto.AnoExpiracao,
                cartaoCriarDto.UltimosDigitos,
                cartaoCriarDto.Padrao);

            var cartao = new Cartao
            {
                IDCliente = cartaoCriarDto.IDCliente,
                BandeiraCartao = cartaoCriarDto.BandeiraCartao,
                UltimosDigitos = cartaoCriarDto.UltimosDigitos,
                MesExpiracao = cartaoCriarDto.MesExpiracao,
                AnoExpiracao = cartaoCriarDto.AnoExpiracao,
                NomeTitular = cartaoCriarDto.NomeTitular,
                Ativo = true,
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
                throw new NotFoundException("Nenhum cartao encontrado com esse IDCartao.");
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

        public async Task<CartaoResponseDto> AtualizarCartaoAsync(int IDCartao, CartaoAtualizarDto cartaoAtualizarDto)
        {
            var cartao = await _cartaoRepository.BuscarCartaoPorIDAsync(IDCartao);

            if (cartao == null)
            {
                throw new NotFoundException("Não existe o cartão com esse ID.");
            }

            await ValidarCartaoAsync(
                cartao.IDCliente,
                cartaoAtualizarDto.MesExpiracao,
                cartaoAtualizarDto.AnoExpiracao,
                cartaoAtualizarDto.UltimosDigitos,
                cartaoAtualizarDto.Padrao,
                IDCartao);

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

        private async Task ValidarCartaoAsync(
            int idCliente,
            int mesExpiracao,
            int anoExpiracao,
            string? ultimosDigitos,
            bool padrao,
            int? idCartaoAtual = null)
        {
            var clienteExiste = await _clienteRepository.BuscarPorIDAsync(idCliente);
            Cartao? cartaoPadrao = await _cartaoRepository.BuscarCartaoPorCliente(idCliente);

            int anoAtual = DateTime.UtcNow.Year;
            int mesAtual = DateTime.UtcNow.Month;

            if (clienteExiste == null)
            {
                throw new NotFoundException("Cliente não existe");
            }

            if (mesExpiracao < 1 || mesExpiracao > 12)
            {
                throw new BusinessException("Mes de expiração do cartão não é valida!");
            }

            if (ultimosDigitos == null || ultimosDigitos.Length != 4 || !ultimosDigitos.All(char.IsDigit))
            {
                throw new BusinessException("O ultimos digitos devem contem 4 digitos e devem ser números!");
            }

            if (anoExpiracao < anoAtual || (anoExpiracao == anoAtual && mesExpiracao < mesAtual))
            {
                throw new BusinessException("O cartão informado já está expirado.");
            }

            if (padrao &&
                cartaoPadrao != null &&
                cartaoPadrao.IDCartao != idCartaoAtual)
            {
                throw new BusinessException("Já existe outro cartão padrão para esse cliente.");
            }
        }

        public async Task<bool> ApagarCartaoAsync(int IDCartao) // Metodo de desabilitar cartao
        {
            var cartao  =  await _cartaoRepository.BuscarCartaoPorIDAsync(IDCartao);

            if (cartao == null)
            {
                throw new NotFoundException("Nenhum cartao encontrado com esse IDCartao.");
            }

            cartao.Ativo = false;
            cartao.Padrao = false;
            
            await _cartaoRepository.SalvarAlteracoesAsync();

            return true;
        }

    }
}
