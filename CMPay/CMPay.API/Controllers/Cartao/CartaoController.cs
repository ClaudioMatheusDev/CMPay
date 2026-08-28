using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CMPay.API.Controllers.Cartao
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartaoController : ControllerBase
    {
        private readonly ICartaoService _cartaoService;

        public CartaoController(ICartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }


        [HttpPost]
        public async Task<IActionResult> CriarCartaoAsync([FromBody] CartaoCriarDto cartaoCriarDto)
        {
            var idCartao = await _cartaoService.CriarCartaoAsync(cartaoCriarDto);
            return Ok(new { IDCartao = idCartao });
        }

        [HttpGet("{IDCartao:int}")]
        public async Task<IActionResult> BuscarCartaoPorIDAsync(int IDCartao)
        {
            var cartao = await _cartaoService.BuscarCartaoPorIDAsync(IDCartao);
            return Ok(cartao);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodosCartoesAsync()
        {
            var cartoes = await _cartaoService.BuscarTodosAsync();
            return Ok(cartoes);
        }


        [HttpDelete("{IDCartao:int}")]
        public async Task<IActionResult> ExcluirCartaoAsync(int IDCartao)
        {
            await _cartaoService.ApagarCartaoAsync(IDCartao);
            return Ok(new { message = "Cartão excluído com sucesso." });
        }

        [HttpPut("{IDCartao:int}")]
        public async Task<IActionResult> AtualizarCartaoAsync(int IDCartao, [FromBody] CartaoAtualizarDto cartaoAtualizarDto)
        {
            await _cartaoService.AtualizarCartaoAsync(IDCartao, cartaoAtualizarDto);
            return Ok(new { message = "Cartão atualizado com sucesso." });
        }
    }
}
