using LojaGerenciamento.Application.Models.ItemPedido;
using LojaGerenciamento.Application.Models.Pagamento;

namespace LojaGerenciamento.Application.Models.Pedido
{
    public class PedidoResponseModel
    {
        public int IdPedido { get; set; }
        public DateTime Data { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Situacao { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorPendente { get; set; }
        public bool EstaQuitado { get; set; }
        public int QuantidadeItens { get; set; }
        public List<ItemPedidoResponseModel> Itens { get; set; }
        public List<PagamentoResponseModel> Pagamentos { get; set; }
    }
}
