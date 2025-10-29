using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure;

/// <summary>
/// Configuração de Dependency Injection para a camada Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configuração do DbContext com PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<BookStoreDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(BookStoreDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });

            // Apenas em desenvolvimento
            if (configuration.GetValue<bool>("UseDetailedErrors"))
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Registro dos repositórios
        services.AddScoped<ILivroRepository, LivroRepository>();
        services.AddScoped<IAutorRepository, AutorRepository>();
        services.AddScoped<IAssuntoRepository, AssuntoRepository>();
        services.AddScoped<IFormaCompraRepository, FormaCompraRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        // Registro do repositório genérico
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }

    /// <summary>
    /// Aplica migrations automaticamente ao iniciar a aplicação
    /// </summary>
    public static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookStoreDbContext>();
        
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Log error - em produção use um logger apropriado
            Console.WriteLine($"Error applying migrations: {ex.Message}");
            throw;
        }
    }
}



