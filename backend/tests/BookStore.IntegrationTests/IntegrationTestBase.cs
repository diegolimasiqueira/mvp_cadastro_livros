using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace BookStore.IntegrationTests;

/// <summary>
/// Classe base para testes de integração com Testcontainers
/// </summary>
public class IntegrationTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    protected readonly WebApplicationFactory<Program> Factory;
    protected HttpClient Client { get; private set; } = null!;

    public IntegrationTestBase()
    {
        // Configurar container PostgreSQL
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("bookstore_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        // Configurar WebApplicationFactory
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    // Remover o DbContext existente
                    services.RemoveAll(typeof(DbContextOptions<BookStoreDbContext>));
                    services.RemoveAll(typeof(BookStoreDbContext));

                    // Adicionar DbContext com connection string do Testcontainer
                    services.AddDbContext<BookStoreDbContext>(options =>
                    {
                        options.UseNpgsql(_postgresContainer.GetConnectionString());
                    });

                    // Aplicar migrations
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
                    db.Database.Migrate();
                });
            });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        Client = Factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
        await Factory.DisposeAsync();
        Client?.Dispose();
    }

    protected async Task<TEntity?> GetEntityFromDatabase<TEntity>(Func<BookStoreDbContext, Task<TEntity?>> query)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
        return await query(dbContext);
    }

    protected async Task ExecuteInDatabase(Func<BookStoreDbContext, Task> action)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
        await action(dbContext);
        await dbContext.SaveChangesAsync();
    }
}



