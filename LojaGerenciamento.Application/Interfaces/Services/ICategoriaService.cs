using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Categoria;

namespace LojaGerenciamento.Application.Interfaces.Services
{
    public interface ICategoriaService
    {
        Task<Response<CategoriaResponseModel>> ObterPorIdAsync(int id);
        Task<Response<ListarCategoriaResponseModel>> ListarCategoriasAsync(FiltroCategoriaRequestModel request);
        Task<Response<CategoriaResponseModel>> CriarCategoriaAsync(CriarCategoriaRequestModel request);
        Task<Response<CategoriaResponseModel>> AtualizarCategoriaAsync(AtualizarCategoriaRequestModel request);
        Task<Response<CategoriaResponseModel>> ExcluirCategoriaAsync(int id);
        Task<Response<CategoriaResponseModel>> AdicionarProdutoAsync(AdicionarProdutoCategoriaRequestModel request);
        Task<Response<CategoriaResponseModel>> RemoverProdutoAsync(RemoverProdutoCategoriaRequestModel request);
    }
}
