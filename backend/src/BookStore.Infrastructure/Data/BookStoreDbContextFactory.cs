using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookStore.Infrastructure.Data;

/// <summary>
/// Factory para criação do DbContext em design time (migrations)
/// </summary>
public class BookStoreDbContextFactory : IDesignTimeDbContextFactory<BookStoreDbContext>
{
    public BookStoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BookStoreDbContext>();
        
        // Connection string padrão para desenvolvimento
        // Em produção, isso virá do appsettings.json
        var connectionString = "Host=localhost;Port=5432;Database=bookstoredb;Username=postgres;Password=postgres123";
        
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(BookStoreDbContext).Assembly.FullName);
        });

        return new BookStoreDbContext(optionsBuilder.Options);
    }
}



