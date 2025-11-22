namespace LojaGerenciamento.Application.Models.Produto
{
    public class AtualizarPrecoProdutoRequestModel
    {
        public int IdProduto { get; set; }
        public decimal NovoPreco { get; set; }
    }

    public class AdicionarEstoqueProdutoRequestModel
    {
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
    }

    public class RemoverEstoqueProdutoRequestModel
    {
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
    }

    public class AtualizarEstoqueProdutoRequestModel
    {
        public int IdProduto { get; set; }
        public int NovoEstoque { get; set; }
    }

}
