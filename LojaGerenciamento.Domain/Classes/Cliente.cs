namespace LojaGerenciamento.Domain.Classes
{
    public class Cliente
    {
        public int IdCliente { get; private set; }
        public string Nome { get; private set; }
        public string? Email { get; private set; }
        public string? Telefone { get; private set; }
        public string Situacao { get; private set; }

        // Verificar como será feita a adição de pedidos ao Cliente
        public ICollection<Pedido> Pedidos { get; private set; } = new List<Pedido>();

        public Cliente() { }

        public Cliente(string nome, string? email, string? telefone)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Situacao = "Ativo";

            Validar();
        }

        private void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new Exception("Nome é obrigatório");
        }

        public void Alterar(string nome, string? email, string? telefone)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;

            Validar();
        }

        public void Excluir()
        {
            if (Situacao == "Excluido")
                throw new Exception("Cliente já está excluído");

            if (Pedidos.Any(p => p.Situacao == "Ativo"))
                throw new Exception("Não é possível excluir cliente com pedidos associados");

            Situacao = "Excluido";
        }

        public decimal ObterValorTotalPedidos()
        {
            // Esse ObterValorTotal() é para ser o mesmo usado em Pedido?
            return Pedidos.Where(p => p.Situacao == "Ativo").Sum(p => p.ObterValorTotal());
        }

        public int ObterQuantidadePedidos()
        {
            return Pedidos.Count(p => p.Situacao == "Ativo");
        }
    }
}
