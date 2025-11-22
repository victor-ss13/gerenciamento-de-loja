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

        public void AdicionarItem(ItemPedido item)
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas pedidos ativos podem ter itens adicionados");
            if (item == null)
                throw new Exception("Item é obrigatório");
            if (item.Quantidade <= 0)
                throw new Exception("Quantidade do item deve ser maior que zero");

            Itens.Add(item);
        }

        public void RemoverItem(int idItemPedido)
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas pedidos ativos podem ter itens removidos");

            var item = Itens.FirstOrDefault(i => i.IdItemPedido == idItemPedido);

            if (item == null)
                throw new Exception("Item não encontrado no pedido");

            Itens.Remove(item);
        }

        public void AtualizarQuantidadeItem(int idItemPedido, int novaQuantidade)
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas pedidos ativos podem ter itens adicionados");

            var item = Itens.FirstOrDefault(i => i.IdItemPedido == idItemPedido);

            if (item == null)
                throw new Exception("Item não encontrado no pedido");

            if (novaQuantidade <= 0)
                throw new Exception("Quantidade do item deve ser maior que zero");

            item.AlterarQuantidade(novaQuantidade);
        }

        public void AdicionarPagamento(Pagamento pagamento)
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas pedidos ativos podem ter pagamentos adicionados");
            if (pagamento == null)
                throw new Exception("Pagamento é obrigatório");

            Pagamentos.Add(pagamento);
        }

        public decimal ObterValorTotal()
        {
            return Itens.Where(i => i.Situacao == "Ativo").Sum(i => i.Preco * i.Quantidade);
        }

        public decimal ObterValorPago()
        {
            return Pagamentos.Where(p => p.Situacao == "Ativo").Sum(p => p.Valor);
        }

        public bool EstaQuitado()
        {
            return ObterValorPago() >= ObterValorTotal();
        }
    }
}
