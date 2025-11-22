namespace LojaGerenciamento.Application.Models.Categoria
{
    public class FiltroCategoriaRequestModel
    {
        public int? IdCategoria { get; set; }
        public string? Nome { get; set; }
        public string? Situacao { get; set; }
        public bool? ApenasCategoriasSemProdutos { get; set; }
    }
}
