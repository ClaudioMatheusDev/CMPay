using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using CMPay.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;

namespace CMPay.API.Controllers.Clientes
{
    [ApiController]
    [Route("api/endereco")]
    public class EnderecoController : Controller
    {

        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarEndereco(EnderecoCriarDto enderecoCriarDto)
        {
            var IDEndereco = await _enderecoService.CriarEnderecoAsync(enderecoCriarDto);

            return Ok(new
            {
                IDEndereco = IDEndereco,
            });
        }

        [HttpGet("{IDEndereco:int}")]
        public async Task<IActionResult> ListarEnderecoPorID(int IDEndereco)
        {
            var endereco = await _enderecoService.BuscarEnderecoPorID(IDEndereco);


            if (endereco == null)
            {
                return NotFound();
            }

            return Ok(endereco);
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodosEnderecos()
        {
            var enderecos = await _enderecoService.BuscarTodosEndereco();

            return Ok(enderecos);
        }

        [HttpPut("{IDEndereco:int}")]
        public async Task<IActionResult> AtualizarEndereco (int IDEndereco, EnderecoAtualizarDto enderecoAtualizarDto)
        {
            var endereco = await _enderecoService.AtualizarEnderecoAsync(IDEndereco, enderecoAtualizarDto);

            return Ok(endereco);
        }

        [HttpDelete("{IDEndereco:int}")]
        public async Task<IActionResult> DeletarEndereco(int IDEndereco)
        {
            var endereco = await _enderecoService.BuscarEnderecoPorID(IDEndereco);


            if (endereco == null)
            {
                return NotFound();
            }

            await _enderecoService.ApagarEnderecoAsync(IDEndereco);

            return Ok();
        }
    }
}
