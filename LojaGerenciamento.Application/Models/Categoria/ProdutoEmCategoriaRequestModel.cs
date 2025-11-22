namespace LojaGerenciamento.Application.Models.Categoria
{
    public class AdicionarProdutoCategoriaRequestModel
    {
        public int IdCategoria { get; set; }
        public int IdProduto { get; set; }
    }

    public class RemoverProdutoCategoriaRequestModel
    {
        public int IdCategoria { get; set; }
        public int IdProduto { get; set; }
    }
}
