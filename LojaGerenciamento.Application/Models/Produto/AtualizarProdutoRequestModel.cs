namespace LojaGerenciamento.Application.Models.Produto
{
    public class AtualizarProdutoRequestModel
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public int IdCategoria { get; set; }
    }
}
