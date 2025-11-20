namespace LojaGerenciamento.Domain.Classes
{
    public class Pagamento
    {
        public int IdPagamento { get; private set; }
        public int IdPedido { get; private set; }
        public Pedido Pedido { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataPagamento { get; private set; } = DateTime.Now;
        public string Metodo { get; private set; }
        public string Situacao { get; private set; }

        public Pagamento() { }

        public Pagamento(Pedido pedido, decimal valor, string metodo)
        {
            Pedido = pedido;
            Valor = valor;
            Metodo = metodo;
            Situacao = "Ativo";

            Validar();
        }

        private void Validar()
        {
            if (Pedido == null)
                throw new Exception("Pedido é obrigátório");
            if (Valor <= 0)
                throw new Exception("Valor inválido");
            if (string.IsNullOrWhiteSpace(Metodo))
                throw new Exception("Método de pagamento é obrigatório");
        }

        // Não imagino que um pagamento deva ser alterado, porém vou manter o método de alterar caso seja necessário futuramente
        public void Alterar(Pedido pedido, decimal valor, string metodo)
        {
            Pedido = pedido;
            Valor = valor;
            Metodo = metodo;

            Validar();
        }

        // Não imagino que pagamentos possam ou devam ser excluídos, mas...
        public void Excluir()
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas pagamentos ativos podem ser excluídos");

            // Restringir exclusão de pagamentos apenas para pagamentos futuros (passados permanecem estáticos)
            if (DataPagamento.Date < DateTime.Now)
                throw new Exception("Não é possível excluir pagamento de data anterior");

            Situacao = "Excluido";
        }
    }
}
