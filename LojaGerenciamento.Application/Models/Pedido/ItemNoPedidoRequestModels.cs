namespace LojaGerenciamento.Application.Models.Pedido
{
    public class AdicionarItemRequestModel
    {
        public int IdPedido { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
    }

    public class AtualizarQuantidadeItemRequestModel
    {
        public int IdPedido { get; set; }
        public int IdItemPedido { get; set; }
        public int NovaQuantidade { get; set; }
    }

    public class RemoverItemRequestModel
    {
        public int IdPedido { get; set; }
        public int IdItemPedido { get; set; }
    }

    public class AdicionarPagamentoRequestModel
    {
        public int IdPedido { get; set; }
        public decimal Valor { get; set; }
        public string Metodo { get; set; }
    }
}
