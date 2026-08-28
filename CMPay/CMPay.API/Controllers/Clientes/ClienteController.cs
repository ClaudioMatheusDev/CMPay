using CMPay.Application.DTOs;
using CMPay.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMPay.API.Controllers.Clientes
{
    [ApiController]
    [Route("api/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CriarCliente([FromBody] ClienteCriarDto clienteCriarDto)
        {
            var resultado = await _clienteService.CriarClienteAsync(clienteCriarDto);
            return Ok(resultado);
        }

        [HttpGet("{IDCliente:int}")]
        public async Task<IActionResult> BuscarClientePorID(int IDCliente)
        {
            var cliente = await _clienteService.BuscarClientePorIDAsync(IDCliente);

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }
        [HttpGet]
        public async Task<IActionResult> BuscarTodosClintes()
        {
            var clientes = await _clienteService.BuscarTodosAsync();

            return Ok(clientes);
        }

        [HttpDelete("{IDCliente:int}")]
        public async Task<IActionResult> DeletarCliente(int IDCliente)
        {
            var cliente = await _clienteService.BuscarClientePorIDAsync(IDCliente);

            if (cliente == null)
            {
                return NotFound();
            }

            await _clienteService.ApagarClienteAsync(IDCliente);

            return Ok();

        }

        [HttpPut("{IDCliente:int}")]
        public async Task<IActionResult> AtualizarCliente(int IDCliente, ClienteAtualizarDto clienteAtualizarDto)
        {
            var cliente = await _clienteService.AtualizarClienteAsync(IDCliente, clienteAtualizarDto);

            return Ok(cliente);
        }

        

    }
}
