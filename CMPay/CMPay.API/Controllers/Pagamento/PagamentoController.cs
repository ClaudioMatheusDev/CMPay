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
            try
            {
                var idPagamento = await _pagamentoService.CriarPagamentoAsync(pagamentoCriarDto);
                return Ok(new { IDPagamento = idPagamento });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarPagamento()
        {
            try
            {
                var pagamentos = await _pagamentoService.ListarPagamentoAsync();
                return Ok(pagamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{IDPagamento:int}")]
        public async Task<IActionResult> ListarPagamentoID(int IDPagamento)
        {
            try
            {
                var pagamento = await _pagamentoService.BuscarPagamentoIDAsync(IDPagamento);
                return Ok(pagamento);
            }
            catch(Exception ex)
            {
                return NotFound(new {message = ex.Message});
            }
        }

        [HttpGet("{IDPagamento:int}/detalhes")]
        public async Task<IActionResult> BuscarDetalhes(int IDPagamento)
        {
            try
            {
                var detalhes =
                    await _pagamentoService.BuscarDetalhesAsync(IDPagamento);

                return Ok(detalhes);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
