using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade Livro no banco de dados
/// </summary>
public class LivroConfiguration : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder.ToTable("Livro");

        builder.HasKey(l => l.CodI);

        builder.Property(l => l.CodI)
            .HasColumnName("CodI")
            .IsRequired();

        builder.Property(l => l.Titulo)
            .HasColumnName("Titulo")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(l => l.Editora)
            .HasColumnName("Editora")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(l => l.Edicao)
            .HasColumnName("Edicao")
            .IsRequired();

        builder.Property(l => l.AnoPublicacao)
            .HasColumnName("AnoPublicacao")
            .HasMaxLength(4)
            .IsRequired();

        // Índices para melhor performance
        builder.HasIndex(l => l.Titulo)
            .HasDatabaseName("IX_Livro_Titulo");
    }
}



