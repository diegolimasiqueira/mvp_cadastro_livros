using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade FormaCompra no banco de dados
/// </summary>
public class FormaCompraConfiguration : IEntityTypeConfiguration<FormaCompra>
{
    public void Configure(EntityTypeBuilder<FormaCompra> builder)
    {
        builder.ToTable("FormaCompra");

        builder.HasKey(fc => fc.CodFc);

        builder.Property(fc => fc.CodFc)
            .HasColumnName("CodFc")
            .IsRequired();

        builder.Property(fc => fc.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(30)
            .IsRequired();

        // Índice único para garantir que não haja formas de compra duplicadas
        builder.HasIndex(fc => fc.Descricao)
            .IsUnique()
            .HasDatabaseName("IX_FormaCompra_Descricao");
    }
}



