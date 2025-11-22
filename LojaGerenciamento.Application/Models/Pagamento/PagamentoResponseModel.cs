namespace LojaGerenciamento.Application.Models.Pagamento
{
    public class PagamentoResponseModel
    {
        public int IdPagamento { get; set; }
        public int IdPedido { get; set; }
        public string Pedido { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataPagamento { get; set; }
        public string Metodo { get; set; }
        public string Situacao { get; set; }
    }
}
