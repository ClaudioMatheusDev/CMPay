using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMPay.API.Controllers.Cartao
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartaoController : Controller
    {
        private readonly ICartaoService _cartaoService;

        public CartaoController(ICartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }


        [HttpPost]
        public async Task<IActionResult> CriarCartaoAsync([FromBody] CartaoCriarDto cartaoCriarDto)
        {
            try
            {
                var idCartao = await _cartaoService.CriarCartaoAsync(cartaoCriarDto);
                return Ok(new { IDCartao = idCartao });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{IDCartao}:int")]
        public async Task<IActionResult> BuscarCartaoPorIDAsync(int IDCartao)
        {
            try
            {
                var cartao = await _cartaoService.BuscarCartaoPorIDAsync(IDCartao);
                return Ok(cartao);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodosCartoesAsync()
        {
            try
            {
                var cartoes = await _cartaoService.BuscarTodosAsync();
                return Ok(cartoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpDelete("{IDCartao}:int")]
        public async Task<IActionResult> ExcluirCartaoAsync(int IDCartao)
        {
            try
            {
                await _cartaoService.ApagarCartaoAsync(IDCartao);
                return Ok(new { message = "Cartão excluído com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{IDCartao}:int")]
        public async Task<IActionResult> AtualizarCartaoAsync(int IDCartao, [FromBody] CartaoAtualizarDto cartaoAtualizarDto)
        {
            try
            {
                await _cartaoService.AtualizarCartaoAsync(IDCartao, cartaoAtualizarDto);
                return Ok(new { message = "Cartão atualizado com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
