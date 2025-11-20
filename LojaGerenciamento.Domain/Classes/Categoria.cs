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

            Situacao = "Excluido";
        }
    }
}
