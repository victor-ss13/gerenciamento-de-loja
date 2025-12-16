using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Pedido;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaGerenciamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] configurar JWT no Program.cs
    [ApiExplorerSettings(IgnoreApi = false)]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        #region CRUD Pedido

        [HttpGet]
        [Route("obter-por-id")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            return Ok(await _pedidoService.ObterPorIdAsync(id));
        }

        [HttpPost]
        [Route("listar")]
        public async Task<IActionResult> ListarPedidosAsync([FromBody] FiltroPedidoRequestModel request)
        {
            return Ok(await _pedidoService.ListarPedidosAsync(request));
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarPedidoAsync([FromBody] CriarPedidoRequestModel request)
        {
            return Ok(await _pedidoService.CriarPedidoAsync(request));
        }

        [HttpPut]
        [Route("atualizar")]
        public async Task<IActionResult> AtualizarPedidoAsync([FromBody] AtualizarPedidoRequestModel request)
        {
            return Ok(await _pedidoService.AtualizarPedidoAsync(request));
        }

        [HttpDelete]
        [Route("excluir")]
        public async Task<IActionResult> ExcluirPedidoAsync(int id)
        {
            return Ok(await _pedidoService.ExcluirPedidoAsync(id));
        }

        #endregion

        #region Itens do Pedido

        [HttpPost]
        [Route("adicionar-item")]
        public async Task<IActionResult> AdicionarItemAsync([FromBody] AdicionarItemRequestModel request)
        {
            return Ok(await _pedidoService.AdicionarItemAsync(request));
        }

        [HttpPost]
        [Route("remover-item")]
        public async Task<IActionResult> RemoverItemAsync([FromBody] RemoverItemRequestModel request)
        {
            return Ok(await _pedidoService.RemoverItemAsync(request));
        }

        [HttpPut]
        [Route("atualizar-quantidade-item")]
        public async Task<IActionResult> AtualizarQuantidadeItemAsync([FromBody] AtualizarQuantidadeItemRequestModel request)
        {
            return Ok(await _pedidoService.AtualizarQuantidadeItemAsync(request));
        }

        #endregion

        #region Pagamentos

        [HttpPost]
        [Route("adicionar-pagamento")]
        public async Task<IActionResult> AdicionarPagamentoAsync([FromBody] AdicionarPagamentoRequestModel request)
        {
            return Ok(await _pedidoService.AdicionarPagamentoAsync(request));
        }

        #endregion

        #region Consultas

        [HttpGet]
        [Route("obter-valor-total")]
        public async Task<IActionResult> ObterValorTotalAsync(int idPedido)
        {
            return Ok(await _pedidoService.ObterValorTotalAsync(idPedido));
        }

        #endregion
    }
}