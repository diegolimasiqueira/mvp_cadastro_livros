using BookStore.Application.DTOs.Health;
using BookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;

namespace BookStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly BookStoreDbContext _context;
    private readonly IConfiguration _configuration;

    public HealthController(BookStoreDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Endpoint básico de health check (compatível com Docker healthcheck)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public IActionResult Get()
    {
        try
        {
            // Tenta fazer uma query simples no banco
            _context.Database.CanConnect();
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
        catch
        {
            return StatusCode(503, new { status = "unhealthy", timestamp = DateTime.UtcNow });
        }
    }

    /// <summary>
    /// Endpoint detalhado de health check com informações do banco de dados
    /// </summary>
    [HttpGet("detailed")]
    [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<HealthCheckResponse>> GetDetailed()
    {
        var response = new HealthCheckResponse
        {
            Status = "Healthy",
            ApiVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0"
        };

        // Obter connection string
        var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
        
        // Parse da connection string
        var dbInfo = ParseConnectionString(connectionString);
        response.Database = dbInfo;

        // Testar conexão com o banco
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            stopwatch.Stop();

            if (canConnect)
            {
                response.Database.IsConnected = true;
                response.Database.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
                
                // Tentar executar uma query simples para confirmar
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SELECT 1");
                }
                catch (Exception ex)
                {
                    response.Database.IsConnected = false;
                    response.Database.ErrorMessage = $"Conexão estabelecida mas falha ao executar query: {ex.Message}";
                    response.Status = "Degraded";
                }
            }
            else
            {
                response.Database.IsConnected = false;
                response.Database.ErrorMessage = "Não foi possível conectar ao banco de dados";
                response.Status = "Unhealthy";
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            response.Database.IsConnected = false;
            response.Database.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            response.Database.ErrorMessage = ex.Message;
            response.Status = "Unhealthy";
        }

        return Ok(response);
    }

    private DatabaseInfo ParseConnectionString(string connectionString)
    {
        var dbInfo = new DatabaseInfo
        {
            Provider = "PostgreSQL"
        };

        try
        {
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim().ToLower();
                    var value = keyValue[1].Trim();

                    switch (key)
                    {
                        case "host":
                        case "server":
                            dbInfo.Host = value;
                            break;
                        case "port":
                            dbInfo.Port = value;
                            break;
                        case "database":
                            dbInfo.DatabaseName = value;
                            break;
                    }
                }
            }

            // Montar URL de conexão (sem senha)
            if (!string.IsNullOrEmpty(dbInfo.Host) && !string.IsNullOrEmpty(dbInfo.Port))
            {
                dbInfo.ConnectionUrl = $"{dbInfo.Host}:{dbInfo.Port}";
                if (!string.IsNullOrEmpty(dbInfo.DatabaseName))
                {
                    dbInfo.ConnectionUrl += $"/{dbInfo.DatabaseName}";
                }
            }
        }
        catch
        {
            dbInfo.ErrorMessage = "Erro ao parsear connection string";
        }

        return dbInfo;
    }
}

