using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaGerenciamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        [Route("obter-por-id")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            return Ok(await _clienteService.ObterPorIdAsync(id));
        }

        [HttpPost]
        [Route("listar")]
        public async Task<IActionResult> ListarClientesAsync([FromBody] FiltroClienteRequestModel request)
        {
            return Ok(await _clienteService.ListarClientesAsync(request));
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarClienteAsync([FromBody] CriarClienteRequestModel request)
        {
            return Ok(await _clienteService.CriarClienteAsync(request));
        }

        [HttpPut]
        [Route("atualizar")]
        public async Task<IActionResult> AtualizarClienteAsync([FromBody] AtualizarClienteRequestModel request)
        {
            return Ok(await _clienteService.AtualizarClienteAsync(request));
        }

        [HttpDelete]
        [Route("excluir")]
        public async Task<IActionResult> ExcluirClienteAsync(int id)
        {
            return Ok(await _clienteService.ExcluirClienteAsync(id));
        }

        [HttpGet]
        [Route("obter-pedidos")]
        public async Task<IActionResult> ObterPedidosDoClienteAsync(int idCliente)
        {
            return Ok(await _clienteService.ObterPedidosDoClienteAsync(idCliente));
        }
    }
}