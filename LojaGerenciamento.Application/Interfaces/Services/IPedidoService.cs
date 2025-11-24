using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Pedido;

namespace LojaGerenciamento.Application.Interfaces.Services
{
    public interface IPedidoService
    {
        // CRUD Básico
        Task<Response<PedidoResponseModel>> ObterPorIdAsync(int id);
        Task<Response<ListarPedidoResponseModel>> ListarPedidosAsync(FiltroPedidoRequestModel request);
        Task<Response<PedidoResponseModel>> CriarPedidoAsync(CriarPedidoRequestModel request);
        Task<Response<PedidoResponseModel>> AtualizarPedidoAsync(AtualizarPedidoRequestModel request);
        Task<Response<PedidoResponseModel>> ExcluirPedidoAsync(int id);

        // Gerenciamento de Itens
        Task<Response<PedidoResponseModel>> AdicionarItemAsync(AdicionarItemRequestModel request);
        Task<Response<PedidoResponseModel>> RemoverItemAsync(RemoverItemRequestModel request);
        Task<Response<PedidoResponseModel>> AtualizarQuantidadeItemAsync(AtualizarQuantidadeItemRequestModel request);

        // Gerenciamento de Pagamentos
        Task<Response<PedidoResponseModel>> AdicionarPagamentoAsync(AdicionarPagamentoRequestModel request);

        // Consultas e Cálculos
        Task<Response<decimal>> ObterValorTotalAsync(int idPedido);
        Task<Response<decimal>> ObterValorPagoAsync(int idPedido);
        Task<Response<bool>> VerificarSeEstaQuitadoAsync(int idPedido);
    }
}
