using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Pagamento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaGerenciamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        [HttpGet]
        [Route("obter-por-id")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            return Ok(await _pagamentoService.ObterPorIdAsync(id));
        }

        [HttpPost]
        [Route("listar")]
        public async Task<IActionResult> ListarPagamentosAsync([FromBody] FiltroPagamentoRequestModel request)
        {
            return Ok(await _pagamentoService.ListarPagamentosAsync(request));
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarPagamentoAsync([FromBody] CriarPagamentoRequestModel request)
        {
            return Ok(await _pagamentoService.CriarPagamentoAsync(request));
        }

        [HttpPut]
        [Route("atualizar")]
        public async Task<IActionResult> AtualizarPagamentoAsync([FromBody] AtualizarPagamentoRequestModel request)
        {
            return Ok(await _pagamentoService.AtualizarPagamentoAsync(request));
        }

        [HttpDelete]
        [Route("excluir")]
        public async Task<IActionResult> ExcluirPagamentoAsync(int id)
        {
            return Ok(await _pagamentoService.ExcluirPagamentoAsync(id));
        }

        [HttpGet]
        [Route("listar-por-pedido")]
        public async Task<IActionResult> ListarPagamentosPorPedidoAsync(int idPedido)
        {
            return Ok(await _pagamentoService.ListarPagamentosPorPedidoAsync(idPedido));
        }

        [HttpGet]
        [Route("obter-total-pago")]
        public async Task<IActionResult> ObterTotalPagoPorPedidoAsync(int idPedido)
        {
            return Ok(await _pagamentoService.ObterTotalPagoPorPedidoAsync(idPedido));
        }
    }
}