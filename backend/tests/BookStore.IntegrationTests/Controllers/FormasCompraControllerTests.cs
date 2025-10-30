using System.Net;
using System.Net.Http.Json;
using BookStore.Application.DTOs.Auth;
using BookStore.Application.DTOs.FormaCompra;
using FluentAssertions;
using Xunit;

namespace BookStore.IntegrationTests.Controllers;

public class FormasCompraControllerTests : IntegrationTestBase
{
    private string? _authToken;

    private async Task<string> GetAuthTokenAsync()
    {
        if (_authToken != null)
            return _authToken;

        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
        _authToken = loginResponse!.Token;

        return _authToken;
    }

    private async Task<HttpClient> GetAuthenticatedClient()
    {
        var token = await GetAuthTokenAsync();
        Client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return Client;
    }

    [Fact]
    public async Task Create_Should_Create_FormaCompra_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var request = new FormaCompraRequestDto { Descricao = "Digital" };

        // Act
        var response = await client.PostAsJsonAsync("/api/FormasCompra", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<FormaCompraResponseDto>();
        created.Should().NotBeNull();
        created!.Descricao.Should().Be("Digital");
    }

    [Fact]
    public async Task GetById_Should_Return_FormaCompra_When_Exists()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var createRequest = new FormaCompraRequestDto { Descricao = "Audiobook" };
        var createResponse = await client.PostAsJsonAsync("/api/FormasCompra", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<FormaCompraResponseDto>();

        // Act
        var response = await client.GetAsync($"/api/FormasCompra/{created!.CodFc}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<FormaCompraResponseDto>();
        result.Should().NotBeNull();
        result!.Descricao.Should().Be("Audiobook");
    }

    [Fact]
    public async Task GetAll_Should_Return_All_FormasCompra()
    {
        // Arrange
        var client = await GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/FormasCompra");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<FormaCompraResponseDto>>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Update_Should_Update_FormaCompra_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var createRequest = new FormaCompraRequestDto { Descricao = "Original" };
        var createResponse = await client.PostAsJsonAsync("/api/FormasCompra", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<FormaCompraResponseDto>();

        var updateRequest = new FormaCompraRequestDto { Descricao = "Atualizado" };

        // Act
        var response = await client.PutAsJsonAsync($"/api/FormasCompra/{created!.CodFc}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<FormaCompraResponseDto>();
        updated!.Descricao.Should().Be("Atualizado");
    }

    [Fact]
    public async Task Delete_Should_Delete_FormaCompra_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var createRequest = new FormaCompraRequestDto { Descricao = "To Delete" };
        var createResponse = await client.PostAsJsonAsync("/api/FormasCompra", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<FormaCompraResponseDto>();

        // Act
        var response = await client.DeleteAsync($"/api/FormasCompra/{created!.CodFc}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await client.GetAsync($"/api/FormasCompra/{created.CodFc}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

