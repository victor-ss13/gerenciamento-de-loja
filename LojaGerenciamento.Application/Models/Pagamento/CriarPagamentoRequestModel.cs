namespace LojaGerenciamento.Application.Models.Pagamento
{
    public class CriarPagamentoRequestModel
    {
        public int IdPedido { get; set; }
        public decimal Valor { get; set; }
        public string Metodo { get; set; }
    }
}
