namespace BookStore.Application.DTOs.Health;

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public DatabaseInfo Database { get; set; } = new();
    public string ApiVersion { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class DatabaseInfo
{
    public string Provider { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ConnectionUrl { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public string? ErrorMessage { get; set; }
    public long ResponseTimeMs { get; set; }
}

