namespace LojaGerenciamento.Application.Models.Cliente
{
    public class ClienteResponseModel
    {
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string Situacao { get; set; }
    }
}
