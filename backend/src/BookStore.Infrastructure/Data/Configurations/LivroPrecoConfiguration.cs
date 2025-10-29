using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade LivroPreco no banco de dados
/// </summary>
public class LivroPrecoConfiguration : IEntityTypeConfiguration<LivroPreco>
{
    public void Configure(EntityTypeBuilder<LivroPreco> builder)
    {
        builder.ToTable("LivroPreco");

        builder.HasKey(lp => lp.CodLp);

        builder.Property(lp => lp.CodLp)
            .HasColumnName("CodLp")
            .IsRequired();

        builder.Property(lp => lp.Livro_CodI)
            .HasColumnName("Livro_CodI")
            .IsRequired();

        builder.Property(lp => lp.FormaCompra_CodFc)
            .HasColumnName("FormaCompra_CodFc")
            .IsRequired();

        builder.Property(lp => lp.Valor)
            .HasColumnName("Valor")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        // Relacionamento com Livro
        builder.HasOne(lp => lp.Livro)
            .WithMany(l => l.LivroPrecos)
            .HasForeignKey(lp => lp.Livro_CodI)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_LivroPreco_Livro");

        // Relacionamento com FormaCompra
        builder.HasOne(lp => lp.FormaCompra)
            .WithMany(fc => fc.LivroPrecos)
            .HasForeignKey(lp => lp.FormaCompra_CodFc)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_LivroPreco_FormaCompra");

        // Índice único para evitar preços duplicados para o mesmo livro e forma de compra
        builder.HasIndex(lp => new { lp.Livro_CodI, lp.FormaCompra_CodFc })
            .IsUnique()
            .HasDatabaseName("IX_LivroPreco_Livro_FormaCompra");

        // Índices para melhor performance
        builder.HasIndex(lp => lp.Livro_CodI)
            .HasDatabaseName("IX_LivroPreco_Livro");

        builder.HasIndex(lp => lp.FormaCompra_CodFc)
            .HasDatabaseName("IX_LivroPreco_FormaCompra");
    }
}



