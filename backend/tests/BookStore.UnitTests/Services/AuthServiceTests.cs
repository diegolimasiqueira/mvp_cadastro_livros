using BookStore.Application.DTOs.Auth;
using BookStore.Application.Exceptions;
using BookStore.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BookStore.UnitTests.Services;

public class AuthServiceTests
{
    private readonly IConfiguration _configuration;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        // Criar configuração real em memória em vez de mockar
        var inMemorySettings = new Dictionary<string, string?>
        {
            {"JwtSettings:SecretKey", "BookStore-SuperSecretKey-2025-MinLength32Chars!"},
            {"JwtSettings:Issuer", "BookStoreAPI"},
            {"JwtSettings:Audience", "BookStoreClient"},
            {"JwtSettings:ExpirationHours", "2"}
        };

        _configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        
        _service = new AuthService(_configuration);
    }

    [Fact]
    public async Task Login_Should_Return_Token_When_Credentials_Are_Valid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Username.Should().Be("admin");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_Should_Throw_UnauthorizedException_When_Username_Is_Invalid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "wronguser",
            Password = "admin123"
        };

        // Act
        var act = async () => await _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid username or password");
    }

    [Fact]
    public async Task Login_Should_Throw_UnauthorizedException_When_Password_Is_Invalid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "admin",
            Password = "wrongpassword"
        };

        // Act
        var act = async () => await _service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid username or password");
    }

    [Fact]
    public async Task Login_Should_Generate_Valid_JWT_Token()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };

        // Act
        var result = await _service.LoginAsync(request);

        // Assert
        result.Token.Should().NotBeNullOrEmpty();
        result.Token.Split('.').Should().HaveCount(3); // JWT has 3 parts: header.payload.signature
    }
}
