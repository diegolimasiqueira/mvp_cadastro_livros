using BookStore.Domain.Entities;
using BookStore.Domain.Entities.Views;
using BookStore.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados para o sistema de cadastro de livros
/// </summary>
public class BookStoreDbContext : DbContext
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
    {
    }

    public DbSet<Livro> Livros => Set<Livro>();
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Assunto> Assuntos => Set<Assunto>();
    public DbSet<LivroAutor> LivroAutores => Set<LivroAutor>();
    public DbSet<LivroAssunto> LivroAssuntos => Set<LivroAssunto>();
    public DbSet<FormaCompra> FormasCompra => Set<FormaCompra>();
    public DbSet<LivroPreco> LivroPrecos => Set<LivroPreco>();
    public DbSet<VwLivrosPorAutor> VwLivrosPorAutor => Set<VwLivrosPorAutor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações
        modelBuilder.ApplyConfiguration(new LivroConfiguration());
        modelBuilder.ApplyConfiguration(new AutorConfiguration());
        modelBuilder.ApplyConfiguration(new AssuntoConfiguration());
        modelBuilder.ApplyConfiguration(new LivroAutorConfiguration());
        modelBuilder.ApplyConfiguration(new LivroAssuntoConfiguration());
        modelBuilder.ApplyConfiguration(new FormaCompraConfiguration());
        modelBuilder.ApplyConfiguration(new LivroPrecoConfiguration());

        // Configurar VIEW para relatórios (a VIEW será criada via migration SQL)
        ConfigureReportView(modelBuilder);
    }

    private void ConfigureReportView(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VwLivrosPorAutor>(entity =>
        {
            entity.HasNoKey(); // VIEW não tem chave primária
            entity.ToView("vw_livros_por_autor"); // Nome da VIEW no banco
        });
    }

    /// <summary>
    /// Garante que o banco de dados seja criado com todas as migrations aplicadas
    /// </summary>
    public async Task EnsureDatabaseCreatedAsync()
    {
        await Database.MigrateAsync();
    }
}



