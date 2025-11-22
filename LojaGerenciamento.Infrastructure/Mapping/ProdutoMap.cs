using LojaGerenciamento.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaGerenciamento.Infrastructure.Mapping
{
    public class ProdutoMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("Produtos");

            builder.HasKey(p => p.IdProduto);
            builder.Property(c => c.IdProduto)
                   .HasColumnName("IdProduto")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Nome).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Preco).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.Estoque).IsRequired();
            builder.Property(p => p.Situacao).IsRequired().HasMaxLength(20);

            builder.HasOne(p => p.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(p => p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict); // evita deletar produtos ao excluir categoria
        }
    }
}
