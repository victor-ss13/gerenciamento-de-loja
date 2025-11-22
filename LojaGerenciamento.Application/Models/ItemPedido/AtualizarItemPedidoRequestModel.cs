namespace LojaGerenciamento.Application.Models.ItemPedido
{
    public class AtualizarItemPedidoRequestModel
    {
        public int IdItemPedido { get; set; }
        public int IdPedido { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}
