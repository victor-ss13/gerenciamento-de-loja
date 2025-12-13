using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Produto;

namespace LojaGerenciamento.Application.Interfaces.Services
{
    public interface IProdutoService
    {
        // CRUD Básico
        Task<Response<ProdutoResponseModel>> ObterPorIdAsync(int id);
        Task<Response<ListarProdutoResponseModel>> ListarProdutosAsync(FiltroProdutoRequestModel request);
        Task<Response<ProdutoResponseModel>> CriarProdutoAsync(CriarProdutoRequestModel request);
        Task<Response<ProdutoResponseModel>> AtualizarProdutoAsync(AtualizarProdutoRequestModel request);
        Task<Response<ProdutoResponseModel>> ExcluirProdutoAsync(int id);

        // Gerenciamento de Preço
        Task<Response<ProdutoResponseModel>> AtualizarPrecoAsync(AtualizarPrecoProdutoRequestModel request);

        // Gerenciamento de Estoque
        Task<Response<ProdutoResponseModel>> AdicionarEstoqueAsync(AdicionarEstoqueProdutoRequestModel request);
        Task<Response<ProdutoResponseModel>> RemoverEstoqueAsync(RemoverEstoqueProdutoRequestModel request);
        Task<Response<ProdutoResponseModel>> AtualizarEstoqueAsync(AtualizarEstoqueProdutoRequestModel request);

        // Consultas de Estoque
        Task<Response<bool>> VerificarEstoqueDisponivelAsync(int idProduto, int quantidade);
        Task<Response<List<ProdutoResponseModel>>> ListarProdutosPorCategoriaAsync(int idCategoria);
    }
}
