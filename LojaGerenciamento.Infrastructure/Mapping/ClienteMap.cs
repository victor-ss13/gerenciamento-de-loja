using LojaGerenciamento.Domain.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LojaGerenciamento.Infrastructure.Mapping
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");

            builder.HasKey(c => c.IdCliente);
            builder.Property(c => c.IdCliente)
                   .HasColumnName("IdCliente")
                   .ValueGeneratedOnAdd();


            builder.Property(c => c.Nome).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Email).HasMaxLength(50);
            builder.Property(c => c.Telefone).HasMaxLength(20);
            builder.Property(c => c.Situacao).IsRequired().HasMaxLength(20);

            builder.HasMany(c => c.Pedidos)
                .WithOne(p => p.Cliente)
                .HasForeignKey(p => p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict); // evita excluir cliente com pedidos
        }
    }
}
