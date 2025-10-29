using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Autor no banco de dados
/// </summary>
public class AutorConfiguration : IEntityTypeConfiguration<Autor>
{
    public void Configure(EntityTypeBuilder<Autor> builder)
    {
        builder.ToTable("Autor");

        builder.HasKey(a => a.CodAu);

        builder.Property(a => a.CodAu)
            .HasColumnName("CodAu")
            .IsRequired();

        builder.Property(a => a.Nome)
            .HasColumnName("Nome")
            .HasMaxLength(40)
            .IsRequired();

        // Índices para melhor performance
        builder.HasIndex(a => a.Nome)
            .HasDatabaseName("IX_Autor_Nome");
    }
}



