using BookStore.Application.Interfaces;
using BookStore.Application.Services;
using BookStore.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Application;

/// <summary>
/// Configuração de Dependency Injection para a camada Application
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registro dos serviços
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILivroService, LivroService>();
        services.AddScoped<IAutorService, AutorService>();
        services.AddScoped<IAssuntoService, AssuntoService>();
        services.AddScoped<IFormaCompraService, FormaCompraService>();
        services.AddScoped<IReportService, ReportService>();

        // Registro dos validadores do FluentValidation
        services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

        return services;
    }
}



