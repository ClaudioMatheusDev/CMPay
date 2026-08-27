using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMPay.API.Controllers.Pagamento
{
    [ApiController]
    [Route("api/pagamento")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPagamento([FromBody] PagamentoCriarDto pagamentoCriarDto)
        {
            var idPagamento = await _pagamentoService.CriarPagamentoAsync(pagamentoCriarDto);
            return Ok(new { IDPagamento = idPagamento });
        }

        [HttpGet]
        public async Task<IActionResult> ListarPagamento()
        {
            var pagamentos = await _pagamentoService.ListarPagamentoAsync();
            return Ok(pagamentos);
        }

        [HttpGet("{IDPagamento:int}")]
        public async Task<IActionResult> ListarPagamentoID(int IDPagamento)
        {
            var pagamento = await _pagamentoService.BuscarPagamentoIDAsync(IDPagamento);
            return Ok(pagamento);
        }

        [HttpGet("{IDPagamento:int}/detalhes")]
        public async Task<IActionResult> BuscarDetalhes(int IDPagamento)
        {
            var detalhes = await _pagamentoService.BuscarDetalhesAsync(IDPagamento);
            return Ok(detalhes);
        }

        [HttpPost("{IDPagamento:int}/estornar")]
        public async Task<IActionResult> EstornarPagamento(int IDPagamento)
        {
            await _pagamentoService.EstornarPagamentoAsync(IDPagamento);

            return Ok(new
            {
                message = "Pagamento estornado com sucesso."
            });
        }

        [HttpPost("{IDPagamento:int}/processar")]
        public async Task<IActionResult> ProcessarPagamento(int IDPagamento)
        {
            await _pagamentoService.ProcessarPagamentoAsync(IDPagamento);
            return Ok(new
            {
                message = "Processamento do pagamento concluído."
            });
        }

        [HttpPost("{IDPagamento:int}/cancelar")]
        public async Task<IActionResult> CancelarPagamento(int IDPagamento)
        {
            await _pagamentoService.CancelarPagamentoAsync(IDPagamento);
            return Ok(new
            {
                message = "Cancelamento do pagamento concluído."
            });
        }
    }
}
