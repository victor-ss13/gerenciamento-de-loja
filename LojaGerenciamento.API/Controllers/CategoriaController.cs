using LojaGerenciamento.Application.Interfaces.Services;
using LojaGerenciamento.Application.Models.Categoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LojaGerenciamento.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        [Route("obter-por-id")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            return Ok(await _categoriaService.ObterPorIdAsync(id));
        }

        [HttpPost]
        [Route("listar")]
        public async Task<IActionResult> ListarCategoriasAsync([FromBody] FiltroCategoriaRequestModel request)
        {
            return Ok(await _categoriaService.ListarCategoriasAsync(request));
        }

        [HttpPost]
        [Route("criar")]
        public async Task<IActionResult> CriarCategoriaAsync([FromBody] CriarCategoriaRequestModel request)
        {
            return Ok(await _categoriaService.CriarCategoriaAsync(request));
        }

        [HttpPut]
        [Route("atualizar")]
        public async Task<IActionResult> AtualizarCategoriaAsync([FromBody] AtualizarCategoriaRequestModel request)
        {
            return Ok(await _categoriaService.AtualizarCategoriaAsync(request));
        }

        [HttpDelete]
        [Route("excluir")]
        public async Task<IActionResult> ExcluirCategoriaAsync(int id)
        {
            return Ok(await _categoriaService.ExcluirCategoriaAsync(id));
        }

        [HttpPost]
        [Route("adicionar-produto")]
        public async Task<IActionResult> AdicionarProdutoAsync([FromBody] AdicionarProdutoCategoriaRequestModel request)
        {
            return Ok(await _categoriaService.AdicionarProdutoAsync(request));
        }

        [HttpPost]
        [Route("remover-produto")]
        public async Task<IActionResult> RemoverProdutoAsync([FromBody] RemoverProdutoCategoriaRequestModel request)
        {
            return Ok(await _categoriaService.RemoverProdutoAsync(request));
        }
    }
}