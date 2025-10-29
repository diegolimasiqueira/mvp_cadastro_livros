namespace BookStore.Application.DTOs.Auth;

/// <summary>
/// DTO para resposta de login
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}



