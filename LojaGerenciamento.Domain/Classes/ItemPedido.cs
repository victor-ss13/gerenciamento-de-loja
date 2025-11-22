namespace LojaGerenciamento.Domain.Classes
{
    public class ItemPedido
    {
        public int IdItemPedido { get; private set; }
        public int IdPedido { get; private set; }
        public Pedido Pedido { get; private set; }
        public int IdProduto { get; private set; }
        public Produto Produto { get; private set; }
        public int Quantidade { get; private set; }
        public decimal Preco { get; private set; }
        public string Situacao { get; private set; }

        public ItemPedido() { }

        public ItemPedido(Pedido pedido, Produto produto, int quantidade, decimal preco)
        {
            Pedido = pedido;
            Produto = produto;
            Quantidade = quantidade;
            Preco = preco;
            Situacao = "Ativo";

            Validar();
        }

        private void Validar()
        {
            if (Pedido == null)
                throw new Exception("Pedido é obrigatório");
            if (Produto == null)
                throw new Exception("Produto é obrigatório");
            if (Quantidade <= 0)
                throw new Exception("Valor inválido para quantidade");
            if (Preco <= 0)
                throw new Exception("Valor inválido para preço");
        }

        public void Alterar(Pedido pedido, Produto produto, int quantidade, decimal preco)
        {
            Pedido = pedido;
            Produto = produto;
            Quantidade = quantidade;
            Preco = preco;

            Validar();
        }

        public void AlterarQuantidade(int novaQuantidade)
        {
            if (novaQuantidade <= 0)
                throw new Exception("Valor inválido para quantidade");

            Quantidade = novaQuantidade;
        }

        public void Excluir()
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas itens ativos podem ser excluídos");

            Situacao = "Excluido";
        }
    }
}
