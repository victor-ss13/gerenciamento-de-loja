namespace LojaGerenciamento.Application.Models.Produto
{
    public class FiltroProdutoRequestModel
    {
        public int? IdProduto { get; set; }
        public string? Nome { get; set; }
        public int? IdCategoria { get; set; }
        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }
        public int? EstoqueMinimo { get; set; }
        public int? EstoqueMaximo { get; set; }
        public bool? ApenasComEstoque { get; set; }
        public bool? ApenasSemEstoque { get; set; }
    }
}
