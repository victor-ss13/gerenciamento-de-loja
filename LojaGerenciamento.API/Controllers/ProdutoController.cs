using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Produto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaGerenciamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        #region CRUD

        [HttpGet]
        [Route("obter-por-id")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            return Ok(await _produtoService.ObterPorIdAsync(id));
        }

        [HttpPost]
        [Route("listar")]
        public async Task<IActionResult> ListarProdutosAsync([FromBody] FiltroProdutoRequestModel request)
        {
            return Ok(await _produtoService.ListarProdutosAsync(request));
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarProdutoAsync([FromBody] CriarProdutoRequestModel request)
        {
            return Ok(await _produtoService.CriarProdutoAsync(request));
        }

        [HttpPut]
        [Route("atualizar")]
        public async Task<IActionResult> AtualizarProdutoAsync([FromBody] AtualizarProdutoRequestModel request)
        {
            return Ok(await _produtoService.AtualizarProdutoAsync(request));
        }

        [HttpDelete]
        [Route("excluir")]
        public async Task<IActionResult> ExcluirProdutoAsync(int id)
        {
            return Ok(await _produtoService.ExcluirProdutoAsync(id));
        }

        #endregion

        #region Gerenciamento de Preço e Estoque

        [HttpPut]
        [Route("atualizar-preco")]
        public async Task<IActionResult> AtualizarPrecoAsync([FromBody] AtualizarPrecoProdutoRequestModel request)
        {
            return Ok(await _produtoService.AtualizarPrecoAsync(request));
        }

        [HttpPut] // Usando PUT pois modifica o estado do produto
        [Route("adicionar-estoque")]
        public async Task<IActionResult> AdicionarEstoqueAsync([FromBody] AdicionarEstoqueProdutoRequestModel request)
        {
            return Ok(await _produtoService.AdicionarEstoqueAsync(request));
        }

        [HttpPut]
        [Route("remover-estoque")]
        public async Task<IActionResult> RemoverEstoqueAsync([FromBody] RemoverEstoqueProdutoRequestModel request)
        {
            return Ok(await _produtoService.RemoverEstoqueAsync(request));
        }

        [HttpPut]
        [Route("atualizar-estoque")]
        public async Task<IActionResult> AtualizarEstoqueAsync([FromBody] AtualizarEstoqueProdutoRequestModel request)
        {
            return Ok(await _produtoService.AtualizarEstoqueAsync(request));
        }

        #endregion

        #region Consultas

        [HttpGet]
        [Route("verificar-estoque")]
        public async Task<IActionResult> VerificarEstoqueDisponivelAsync(int idProduto, int quantidade)
        {
            return Ok(await _produtoService.VerificarEstoqueDisponivelAsync(idProduto, quantidade));
        }

        [HttpGet]
        [Route("listar-por-categoria")]
        public async Task<IActionResult> ListarProdutosPorCategoriaAsync(int idCategoria)
        {
            return Ok(await _produtoService.ListarProdutosPorCategoriaAsync(idCategoria));
        }

        #endregion
    }
}