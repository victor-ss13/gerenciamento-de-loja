namespace LojaGerenciamento.Domain.Classes
{
    public class Produto
    {
        public int IdProduto { get; private set; }
        public string Nome { get; private set; }
        public decimal Preco { get; private set; }
        public int Estoque { get; private set; }

        public int IdCategoria { get; private set; }
        public Categoria Categoria { get; private set; }

        public string Situacao { get; private set; }

        public Produto() { }

        public Produto(string nome, decimal preco, int estoque, Categoria categoria)
        {
            Nome = nome;
            Preco = preco;
            Estoque = estoque;
            Categoria = categoria;
            Situacao = "Ativo";

            Validar();
        }

        private void Validar()
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new Exception("Nome do produto é obrigatório");

            if (Preco <= 0)
                throw new Exception("Preço é obrigatório");

            if (Estoque <= 0)
                throw new Exception("Estoque é obrigatório");

            if (Categoria == null)
                throw new Exception("Categoria do produto é obrigatória");
        }

        public void Alterar(string nome, decimal preco, int estoque, Categoria categoria)
        {
            Nome = nome;
            Preco = preco;
            Estoque = estoque;
            Categoria = categoria;

            Validar();
        }

        public void AdicionarEstoque(int quantidade)
        {
            if (Situacao != "Ativo")
                throw new Exception("Não é possível adicionar estoque a um produto inativo");
            if (quantidade <= 0)
                throw new Exception("Quantidadede deve ser maior que zero");

            Estoque += quantidade;
        }

        public void RemoverEstoque(int quantidade)
        {
            if (Situacao != "Ativo")
                throw new Exception("Não é possível remover estoque de um produto inativo");
            if (quantidade <= 0)
                throw new Exception("Quantidadede deve ser maior que zero");
            if (Estoque < quantidade)
                throw new Exception("Estoque insuficiente");

            Estoque -= quantidade;
        }

        public void AtualizarEstoque(int novoEstoque)
        {
            if (novoEstoque < 0)
                throw new Exception("Estoque não pode ser negativo");

            Estoque = novoEstoque;
        }

        public bool TemEstoqueDisponivel(int quantidade)
        {
            return Estoque >= quantidade;
        }

        public void Excluir()
        {
            if (Situacao != "Ativo")
                throw new Exception("Apenas produtos ativos podem ser excluídos");

            if (Estoque > 0)
                throw new Exception("Não é possível excluir produto com estoque disponível");

            Situacao = "Excluido";
        }
    }
}
