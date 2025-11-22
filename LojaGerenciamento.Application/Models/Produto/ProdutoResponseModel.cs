using Microsoft.Identity.Client;

namespace LojaGerenciamento.Application.Models.Produto
{
    public class ProdutoResponseModel
    {
        public int IdProduto { get; set; }
        public string Nome { get; set; }
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; }
        public string Situacao { get; set; }
        public bool TemEstoque { get; set; }
    }
}
