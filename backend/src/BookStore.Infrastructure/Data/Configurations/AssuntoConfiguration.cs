using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Assunto no banco de dados
/// </summary>
public class AssuntoConfiguration : IEntityTypeConfiguration<Assunto>
{
    public void Configure(EntityTypeBuilder<Assunto> builder)
    {
        builder.ToTable("Assunto");

        builder.HasKey(a => a.CodAs);

        builder.Property(a => a.CodAs)
            .HasColumnName("CodAs")
            .IsRequired();

        builder.Property(a => a.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(20)
            .IsRequired();

        // Índices para melhor performance
        builder.HasIndex(a => a.Descricao)
            .HasDatabaseName("IX_Assunto_Descricao");
    }
}



