
namespace LojaGerenciamento.Application.Models.Cliente
{
    public class AtualizarClienteRequestModel
    {
        public int IdCliente { get; set; }
        public required string Nome { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
    }
}
