namespace BookStore.Application.DTOs.Auth;

/// <summary>
/// DTO para requisição de login
/// </summary>
public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}



