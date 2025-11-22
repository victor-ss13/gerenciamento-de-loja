namespace LojaGerenciamento.Application.Models.Cliente
{
    public class ClienteResponseModel
    {
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string Situacao { get; set; }
        public int QuantidadePedidos { get; set; }
        public decimal ValorTotalPedidos { get; set; }
        public List<PedidoResumoClienteResponseModel> Pedidos { get; set; }
    }

    public class PedidoResumoClienteResponseModel
    {
        public int IdPedido { get; set; }
        public DateTime Data { get; set; }
        public decimal ValorTotal { get; set; }
        public bool EstaQuitado { get; set; }
        public string Situacao { get; set; }
}
