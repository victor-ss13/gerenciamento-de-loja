namespace LojaGerenciamento.Application.Models.Produto
{
    public class CriarProdutoRequestModel
    {
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public int IdCategoria { get; set; }
    }
}
