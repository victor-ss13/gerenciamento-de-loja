namespace LojaGerenciamento.Application.Models.Cliente
{
    public class FiltroClienteRequestModel
    {
        public int? IdCliente { get; set; }
        public string? Nome { get; set; }
        public bool? ApenasComPedidos { get; set; }
        public bool? ApenasSemPedidos { get; set; }
    }
}
