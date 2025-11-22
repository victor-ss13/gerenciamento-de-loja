using LojaGerenciamento.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaGerenciamento.Infrastructure.Mapping
{
    public class PagamentoMap : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.ToTable("Pagamentos");

            builder.HasKey(p => p.IdPagamento);

            builder.Property(p => p.IdPagamento)
                   .HasColumnName("IdPagamento")
                   .ValueGeneratedOnAdd();

            builder.Property(p => p.Valor).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.DataPagamento).IsRequired().HasColumnType("datetime");
            builder.Property(p => p.Metodo).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Situacao).IsRequired().HasMaxLength(20);

            builder.HasOne(p => p.Pedido)
                .WithMany(ped => ped.Pagamentos)
                .HasForeignKey(p => p.IdPedido)
                .OnDelete(DeleteBehavior.Cascade); // ao excluir pedido, pagamentos também são removidos
        }
    }
}
