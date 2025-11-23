namespace LojaGerenciamento.Domain.Classes
{
    public class Categoria
    {
        public int IdCategoria { get; private set; }
        public string Nome { get; private set; }
        public string Situacao { get; private set; }

        // Verificar como será feita adição de produtos à categoria
        public ICollection<Produto> Produtos { get; private set; }

        public Categoria() { }

        public Categoria(string nome)
        {
            Nome = nome;
            Situacao = "Ativo";

            Validar();
        }

        private void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new Exception("Nome da categoria é obrigatório");
        }

        public void Alterar(string nome)
        {
            Nome = nome;

            Validar();
        }

        public void Excluir()
        {
            if (Situacao == "Excluido")
                throw new Exception("Categoria já está excluída");
            if (Produtos.Any(p => p.Situacao == "Ativo"))
                throw new Exception("Não é possível excluir categoria com produtos ativos associados");

            Situacao = "Excluido";
        }

        public void AdicionarProduto(Produto produto)
        {
            if (Situacao != "Ativo")
                throw new Exception("Não é possível adicionar produtos à uma categoria inativa");
            if (produto == null)
                throw new Exception("Produto é obrigatório");

            Produtos.Add(produto);
        }

        public void RemoverProduto(int idProduto)
        {
            if (Situacao != "Ativo")
                throw new Exception("Não é possível adicionar produtos à uma categoria inativa");

            var produto = Produtos.FirstOrDefault(p => p.IdProduto == idProduto);

            if (produto == null)
                throw new Exception("Produto não encontrado na categoria");

            Produtos.Remove(produto);
        }
    }
}
