namespace LojaGerenciamento.Application.Models.ItemPedido
{
    public class CriarItemPedidoRequestModel
    {
        public int IdPedido { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }
}
