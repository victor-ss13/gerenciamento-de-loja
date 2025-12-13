using LojaGerenciamento.Application.Models;
using LojaGerenciamento.Application.Models.Cliente;

namespace LojaGerenciamento.Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task<Response<ClienteResponseModel>> ObterPorIdAsync(int id);
        Task<Response<ListarClienteResponseModel>> ListarClientesAsync(FiltroClienteRequestModel request);
        Task<Response<ClienteResponseModel>> CriarClienteAsync(CriarClienteRequestModel request);
        Task<Response<ClienteResponseModel>> AtualizarClienteAsync(AtualizarClienteRequestModel request);
        Task<Response<ClienteResponseModel>> ExcluirClienteAsync(int id);
        Task<Response<List<PedidoResumoClienteResponseModel>>> ObterPedidosDoClienteAsync(int idCliente);
    }
}
