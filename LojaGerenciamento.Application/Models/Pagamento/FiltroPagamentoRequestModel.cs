namespace LojaGerenciamento.Application.Models.Pagamento
{
    public class FiltroPagamentoRequestModel
    {
        public int? IdPagamento { get; set; }
        public int? IdPedido { get; set; }
        public decimal? ValorMinimo { get; set; }
        public decimal? ValorMaximo { get; set; }
        public DateTime? DataPagamentoInicio { get; set; }
        public DateTime? DataPagamentoFim { get; set; }
        public string? Metodo { get; set; }
    }
}
