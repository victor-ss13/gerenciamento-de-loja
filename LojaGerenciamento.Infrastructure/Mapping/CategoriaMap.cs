using LojaGerenciamento.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaGerenciamento.Infrastructure.Mapping
{
    public class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");

            builder.HasKey(c => c.IdCategoria);
            builder.Property(c => c.IdCategoria)
                   .HasColumnName("IdCategoria")
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.Nome).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Situacao).IsRequired().HasMaxLength(20);

            builder.HasMany(c => c.Produtos)
                .WithOne(p => p.Categoria)
                .HasForeignKey(p => p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict); // evita deletar produtos junto de categorias
        }
    }
}
