using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Pagamento;

namespace LojaGerenciamento.Application.Interfaces.Services
{
    public interface IPagamentoService
    {
        // CRUD Básico
        Task<Response<PagamentoResponseModel>> ObterPorIdAsync(int id);
        Task<Response<ListarPagamentoResponseModel>> ListarPagamentosAsync(FiltroPagamentoRequestModel request);
        Task<Response<PagamentoResponseModel>> CriarPagamentoAsync(CriarPagamentoRequestModel request);
        Task<Response<PagamentoResponseModel>> AtualizarPagamentoAsync(AtualizarPagamentoRequestModel request);
        Task<Response<PagamentoResponseModel>> ExcluirPagamentoAsync(int id);

        // Consultas específicas
        Task<Response<List<PagamentoResponseModel>>> ListarPagamentosPorPedidoAsync(int idPedido);
        Task<Response<decimal>> ObterTotalPagoPorPedidoAsync(int idPedido);
    }
}
