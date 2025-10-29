using BookStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração da entidade de relacionamento LivroAssunto no banco de dados
/// </summary>
public class LivroAssuntoConfiguration : IEntityTypeConfiguration<LivroAssunto>
{
    public void Configure(EntityTypeBuilder<LivroAssunto> builder)
    {
        builder.ToTable("Livro_Assunto");

        // Chave composta
        builder.HasKey(la => new { la.Livro_CodI, la.Assunto_CodAs });

        builder.Property(la => la.Livro_CodI)
            .HasColumnName("Livro_CodI")
            .IsRequired();

        builder.Property(la => la.Assunto_CodAs)
            .HasColumnName("Assunto_CodAs")
            .IsRequired();

        // Relacionamento com Livro
        builder.HasOne(la => la.Livro)
            .WithMany(l => l.LivroAssuntos)
            .HasForeignKey(la => la.Livro_CodI)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Livro_Assunto_Livro");

        // Relacionamento com Assunto
        builder.HasOne(la => la.Assunto)
            .WithMany(a => a.LivroAssuntos)
            .HasForeignKey(la => la.Assunto_CodAs)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Livro_Assunto_Assunto");

        // Índices conforme modelo
        builder.HasIndex(la => la.Livro_CodI)
            .HasDatabaseName("Livro_Assunto_FKIndex1");

        builder.HasIndex(la => la.Assunto_CodAs)
            .HasDatabaseName("Livro_Assunto_FKIndex2");
    }
}



