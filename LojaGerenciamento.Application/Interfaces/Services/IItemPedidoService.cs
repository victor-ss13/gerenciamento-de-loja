using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.ItemPedido;

namespace LojaGerenciamento.Application.Interfaces.Services
{
    public interface IItemPedidoService
    {
        Task<Response<ItemPedidoResponseModel>> ObterPorIdAsync(int id);
        Task<Response<ListarItemPedidoResponseModel>> ListarItensAsync(FiltroItemPedidoRequestModel request);
        Task<Response<ItemPedidoResponseModel>> CriarItemAsync(CriarItemPedidoRequestModel request);
        Task<Response<ItemPedidoResponseModel>> AtualizarItemAsync(AtualizarItemPedidoRequestModel request);
        Task<Response<ItemPedidoResponseModel>> AtualizarQuantidadeAsync(int idItem, int novaQuantidade);
        Task<Response<ItemPedidoResponseModel>> ExcluirItemAsync(int id);
        Task<Response<List<ItemPedidoResponseModel>>> ListarItensPorPedidoAsync(int idPedido);
    }
}
