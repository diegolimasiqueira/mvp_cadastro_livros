using System.Net;
using System.Net.Http.Json;
using BookStore.Application.DTOs.Auth;
using FluentAssertions;
using Xunit;

namespace BookStore.IntegrationTests.Controllers;

public class AuthControllerTests : IntegrationTestBase
{
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
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.Username.Should().Be("admin");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Username_Is_Invalid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "invaliduser",
            Password = "admin123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_Return_Unauthorized_When_Password_Is_Invalid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "admin",
            Password = "wrongpassword"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_Should_Return_BadRequest_When_Request_Is_Invalid()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Username = "", // Username vazio
            Password = "admin123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Token_Should_Allow_Access_To_Protected_Endpoints()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

        Client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        // Act
        var response = await Client.GetAsync("/api/autores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Protected_Endpoint_Should_Return_Unauthorized_Without_Token()
    {
        // Act
        var response = await Client.GetAsync("/api/autores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

