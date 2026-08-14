using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;
using CMPay.Domain.Enums;

namespace CMPay.Application.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IProcessadorPagamento _processadorPagamento;
        public PagamentoService
            (IPagamentoRepository pagamentoRepository,
            IClienteRepository clienteRepository,
            ITransacaoRepository transacaoRepository,
            IProcessadorPagamento processadorPagamento)
        {
            _pagamentoRepository = pagamentoRepository;
            _clienteRepository = clienteRepository;
            _transacaoRepository = transacaoRepository;
            _processadorPagamento = processadorPagamento;
        }

        private decimal ObterPercentualTaxa(TipoMetodoPagamento tipoMetodoPagamento)
        {
            return tipoMetodoPagamento switch
            {
                TipoMetodoPagamento.Pix => 0.01m,
                TipoMetodoPagamento.CartaoCredito => 0.03m,
                TipoMetodoPagamento.CartaoDebito => 0.02m,
                _ => throw new Exception("Método de pagamento inválido.")
            };
        }

        public async Task<int> CriarPagamentoAsync(PagamentoCriarDto pagamentoCriarDto)
        {
            var clienteExiste =
                await _clienteRepository.BuscarPorIDAsync(pagamentoCriarDto.IDCliente);

            if (clienteExiste == null)
            {
                throw new Exception("Não existe um cliente com esse ID.");
            }

            if (pagamentoCriarDto.ValorBruto <= 0)
            {
                throw new Exception("Valor não pode ser igual ou menor que ZERO.");
            }

            var percentualTaxa =
                ObterPercentualTaxa(pagamentoCriarDto.TipoMetodoPagamento);

            var valorTaxa = Math.Round(
                pagamentoCriarDto.ValorBruto * percentualTaxa,
                2,
                MidpointRounding.AwayFromZero);

            var valorLiquido = Math.Round(
                pagamentoCriarDto.ValorBruto - valorTaxa,
                2,
                MidpointRounding.AwayFromZero);

            var pagamento = new Pagamento
            {
                IDCliente = pagamentoCriarDto.IDCliente,
                ValorBruto = pagamentoCriarDto.ValorBruto,
                ValorTaxa = valorTaxa,
                ValorLiquido = valorLiquido,
                Moeda = pagamentoCriarDto.Moeda,
                TipoMetodoPagamento = pagamentoCriarDto.TipoMetodoPagamento,
                StatusPagamento = StatusPagamento.Pendente,
                DataCriacao = DateTime.UtcNow
            };

            await _pagamentoRepository.AdicionarAsync(pagamento);
            await _pagamentoRepository.SalvarAlteracoesAsync();

            var transacao = new Transacao
            {
                IDPagamento = pagamento.IDPagamento,
                TipoTransacao = TipoTransacao.Pagamento,
                StatusTransacao = StatusTransacao.Processando,
                Valor = pagamento.ValorBruto,
                DataTransacao = DateTime.UtcNow
            };


            var resultado = await _processadorPagamento.ProcessarAsync(pagamento);

            if (resultado.Aprovado)
            {
                pagamento.StatusPagamento = StatusPagamento.Aprovado;
                pagamento.DataPagamento = DateTime.UtcNow;

                transacao.StatusTransacao = StatusTransacao.Sucesso;
            }
            else
            {
                pagamento.StatusPagamento = StatusPagamento.Rejeitado;

                transacao.StatusTransacao = StatusTransacao.Falha;
            }

            transacao.Mensagem = resultado.Mensagem;


            await _transacaoRepository.AdicionarAsync(transacao);
            await _transacaoRepository.SalvarAlteracoesAsync();



            return pagamento.IDPagamento;
        }

        public async Task<PagamentoResponseDto> BuscarPagamentoIDAsync(int IDPagamento)
        {
            var pagamento = await _pagamentoRepository.BuscarPorIDAsync(IDPagamento);

            if (pagamento == null)
            {
                throw new Exception("Não foi encontrado nenhum pagamento com esse ID.");
            }

            return new PagamentoResponseDto
            {
                IDPagamento = pagamento.IDPagamento,
                IDCliente = pagamento.IDCliente,
                ValorBruto = pagamento.ValorBruto,
                ValorTaxa = pagamento.ValorTaxa,
                ValorLiquido = pagamento.ValorLiquido,
                Moeda = pagamento.Moeda,
                TipoMetodoPagamento = pagamento.TipoMetodoPagamento,
                StatusPagamento = pagamento.StatusPagamento,
                DataCriacao = pagamento.DataCriacao,
                DataPagamento = pagamento.DataPagamento,
                DataCancelamento = pagamento.DataCancelamento,
                DataEstorno = pagamento.DataEstorno
            };

        }

        public async Task<List<PagamentoResponseDto>> ListarPagamentoAsync()
        {
            var pagamento = await _pagamentoRepository.BuscarTodosAsync();

            return pagamento.Select(pagamento => new PagamentoResponseDto
            {
                IDPagamento = pagamento.IDPagamento,
                IDCliente = pagamento.IDCliente,
                ValorBruto = pagamento.ValorBruto,
                ValorTaxa = pagamento.ValorTaxa,
                ValorLiquido = pagamento.ValorLiquido,
                Moeda = pagamento.Moeda,
                TipoMetodoPagamento = pagamento.TipoMetodoPagamento,
                StatusPagamento = pagamento.StatusPagamento,
                DataCriacao = pagamento.DataCriacao,
                DataPagamento = pagamento.DataPagamento,
                DataCancelamento = pagamento.DataCancelamento,
                DataEstorno = pagamento.DataEstorno
            }).ToList();
        }
        public async Task<PagamentoDetalheDto> BuscarDetalhesAsync(int IDPagamento)
        {
            var pagamento = await _pagamentoRepository.BuscarPorIDAsync(IDPagamento);

            if (pagamento == null)
            {
                throw new Exception("Não foi encontrado nenhum pagamento com esse ID.");
            }

            var transacoes =
                await _transacaoRepository.BuscarPorPagamentoAsync(IDPagamento);

            var transacoesDto = transacoes
                .Select(t => new TransacaoResponseDto
                {
                    IDTransacao = t.IDTransacao,
                    IDPagamento = t.IDPagamento,
                    TipoTransacao = t.TipoTransacao,
                    StatusTransacao = t.StatusTransacao,
                    Mensagem = t.Mensagem,
                    Valor = t.Valor,
                    DataTransacao = t.DataTransacao
                })
                .ToList();

            return new PagamentoDetalheDto
            {
                IDPagamento = pagamento.IDPagamento,
                IDCliente = pagamento.IDCliente,
                ValorBruto = pagamento.ValorBruto,
                ValorTaxa = pagamento.ValorTaxa,
                ValorLiquido = pagamento.ValorLiquido,
                Moeda = pagamento.Moeda,
                TipoMetodoPagamento = pagamento.TipoMetodoPagamento,
                StatusPagamento = pagamento.StatusPagamento,
                DataCriacao = pagamento.DataCriacao,
                DataPagamento = pagamento.DataPagamento,
                DataCancelamento = pagamento.DataCancelamento,
                DataEstorno = pagamento.DataEstorno,

                Transacoes = transacoesDto
            };
        }
    }
}
