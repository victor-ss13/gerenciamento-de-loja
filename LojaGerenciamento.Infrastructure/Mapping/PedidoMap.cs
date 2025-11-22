using LojaGerenciamento.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaGerenciamento.Infrastructure.Mapping
{
    public class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(p => p.IdPedido);

            builder.Property(p => p.IdPedido)
                   .HasColumnName("IdPedido")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Data).IsRequired().HasColumnType("datetime");
            builder.Property(p => p.Situacao).IsRequired().HasMaxLength(20);

            builder.HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.IdPedido)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Pagamentos)
                .WithOne(pag => pag.Pedido)
                .HasForeignKey(pag => pag.IdPedido)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
