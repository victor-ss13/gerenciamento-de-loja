namespace LojaGerenciamento.Application.Models.Pedido
{
    public class FiltroPedidoRequestModel
    {
        public int? IdPedido { get; set; }
        public int? IdCliente { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public decimal? ValorTotalMinimo { get; set; }
        public decimal? ValorTotalMaximo { get; set; }
        public bool? ApenasQuitados { get; set; }
        public bool? ApenasPendentes { get; set; }
    }
}
