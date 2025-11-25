using LojaGerenciamento.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaGerenciamento.Infrastructure.Mapping
{
    public class ItemPedidoMap : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> builder)
        {
            builder.ToTable("ItemPedido");

            builder.HasKey(ip => ip.IdItemPedido);

            builder.Property(ip => ip.IdItemPedido)
                   .HasColumnName("IdItemPedido")
                   .ValueGeneratedOnAdd();

            builder.Property(ip => ip.Quantidade).IsRequired();
            builder.Property(ip => ip.Preco).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(ip => ip.Situacao).IsRequired().HasMaxLength(20);

            builder.HasOne(ip => ip.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(ip => ip.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
