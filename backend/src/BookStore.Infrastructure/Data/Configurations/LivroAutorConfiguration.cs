using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade de relacionamento LivroAutor no banco de dados
/// </summary>
public class LivroAutorConfiguration : IEntityTypeConfiguration<LivroAutor>
{
    public void Configure(EntityTypeBuilder<LivroAutor> builder)
    {
        builder.ToTable("Livro_Autor");

        // Chave composta
        builder.HasKey(la => new { la.Livro_CodI, la.Autor_CodAu });

        builder.Property(la => la.Livro_CodI)
            .HasColumnName("Livro_CodI")
            .IsRequired();

        builder.Property(la => la.Autor_CodAu)
            .HasColumnName("Autor_CodAu")
            .IsRequired();

        // Relacionamento com Livro
        builder.HasOne(la => la.Livro)
            .WithMany(l => l.LivroAutores)
            .HasForeignKey(la => la.Livro_CodI)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Livro_Autor_Livro");

        // Relacionamento com Autor
        builder.HasOne(la => la.Autor)
            .WithMany(a => a.LivroAutores)
            .HasForeignKey(la => la.Autor_CodAu)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Livro_Autor_Autor");

        // Índices conforme modelo
        builder.HasIndex(la => la.Livro_CodI)
            .HasDatabaseName("Livro_Autor_FKIndex1");

        builder.HasIndex(la => la.Autor_CodAu)
            .HasDatabaseName("Livro_Autor_FKIndex2");
    }
}



