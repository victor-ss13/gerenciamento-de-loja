namespace LojaGerenciamento.Application.Models.ItemPedido
{
    public class ItemPedidoResponseModel
    {
        public int IdItemPedido { get; set; }
        public int IdPedido { get; set; }
        public string Pedido { get; set; }
        public int IdProduto { get; set; }
        public string Produto { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}
