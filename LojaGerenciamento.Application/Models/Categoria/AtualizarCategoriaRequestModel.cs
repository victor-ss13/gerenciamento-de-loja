namespace LojaGerenciamento.Application.Models.Categoria
{
    public class AtualizarCategoriaRequestModel
    {
        public int IdCategoria { get; set; }
        public required string Nome { get; set; }
    }
}
