using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookStore.Application.DTOs.Auth;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BookStore.Application.Services;

/// <summary>
/// Serviço de autenticação com JWT
/// </summary>
public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Validar credenciais (usuário fixo conforme requisito)
        if (request.Username != "admin" || request.Password != "admin123")
        {
            throw new UnauthorizedException("Invalid username or password");
        }

        // Gerar token JWT
        var token = GenerateJwtToken(request.Username);
        var expirationHours = _configuration.GetValue<int>("JwtSettings:ExpirationHours", 2);

        return await Task.FromResult(new LoginResponseDto
        {
            Token = token,
            Username = request.Username,
            ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
        });
    }

    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var issuer = jwtSettings["Issuer"] ?? "BookStoreAPI";
        var audience = jwtSettings["Audience"] ?? "BookStoreClient";
        var expirationHours = jwtSettings.GetValue<int>("ExpirationHours", 2);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}



