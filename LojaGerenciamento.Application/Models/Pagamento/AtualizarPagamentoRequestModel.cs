namespace LojaGerenciamento.Application.Models.Pagamento
{
    public class AtualizarPagamentoRequestModel
    {
        public int IdPagamento { get; set; }
        public int IdPedido { get; set; }
        public decimal Valor { get; set; }
        public string Metodo { get; set; }
    }
}
