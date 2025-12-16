using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.ItemPedido;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaGerenciamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ItemPedidoController : ControllerBase
    {
        private readonly IItemPedidoService _itemPedidoService;

        public ItemPedidoController(IItemPedidoService itemPedidoService)
        {
            _itemPedidoService = itemPedidoService;
        }

        [HttpGet]
        [Route("obter-por-id")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            return Ok(await _itemPedidoService.ObterPorIdAsync(id));
        }

        [HttpPost]
        [Route("listar")]
        public async Task<IActionResult> ListarItensAsync([FromBody] FiltroItemPedidoRequestModel request)
        {
            return Ok(await _itemPedidoService.ListarItensAsync(request));
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarItemAsync([FromBody] CriarItemPedidoRequestModel request)
        {
            return Ok(await _itemPedidoService.CriarItemAsync(request));
        }

        [HttpPut]
        [Route("atualizar")]
        public async Task<IActionResult> AtualizarItemAsync([FromBody] AtualizarItemPedidoRequestModel request)
        {
            return Ok(await _itemPedidoService.AtualizarItemAsync(request));
        }

        [HttpPut]
        [Route("atualizar-quantidade")]
        public async Task<IActionResult> AtualizarQuantidadeAsync(int idItem, int novaQuantidade)
        {
            // Nota: idItem e novaQuantidade virão na Query String da URL
            return Ok(await _itemPedidoService.AtualizarQuantidadeAsync(idItem, novaQuantidade));
        }

        [HttpDelete]
        [Route("excluir")]
        public async Task<IActionResult> ExcluirItemAsync(int id)
        {
            return Ok(await _itemPedidoService.ExcluirItemAsync(id));
        }

        [HttpGet]
        [Route("listar-por-pedido")]
        public async Task<IActionResult> ListarItensPorPedidoAsync(int idPedido)
        {
            return Ok(await _itemPedidoService.ListarItensPorPedidoAsync(idPedido));
        }
    }
}