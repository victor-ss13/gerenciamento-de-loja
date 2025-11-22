namespace LojaGerenciamento.Domain.Classes
{
    public class Pedido
    {
        public int IdPedido { get; private set; }
        public DateTime Data { get; private set; } = DateTime.Now;
        public int IdCliente { get; private set; }
        public Cliente Cliente { get; private set; }
        public string Situacao { get; private set; }
        public ICollection<ItemPedido> Itens { get; private set; } = new List<ItemPedido>();
        public ICollection<Pagamento> Pagamentos { get; private set; } = new List<Pagamento>();

        public Pedido() { }

        public Pedido(Cliente cliente)
        {
            Cliente = cliente;
            Situacao = "Ativo";

            Validar();
        }

        private void Validar()
        {
            if (Cliente == null)
                throw new Exception("Cliente é obrigatório");
        }

        public void Alterar(Cliente cliente)
        {
            Cliente = cliente;

            Validar();
        }

        public void Excluir()
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas pedidos ativos podem ser excluídos");

            Situacao = "Excluido";
        }
    }
}
