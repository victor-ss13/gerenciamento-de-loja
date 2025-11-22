namespace LojaGerenciamento.Application.Models.Categoria
{
    public class CategoriaResponseModel
    {
        public int IdCategoria { get; set; }
        public string Nome { get; set; }
        public string Situacao { get; set; }
        public int QuantidadeProdutos { get; set; }
        public List<ProdutoResumoResponseModel> Produtos { get; set; }
    }

    public class ProdutoResumoResponseModel
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }
}
