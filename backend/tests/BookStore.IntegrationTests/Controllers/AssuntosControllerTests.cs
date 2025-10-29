using System.Net;
using System.Net.Http.Json;
using BookStore.Application.DTOs.Assunto;
using BookStore.Application.DTOs.Auth;
using FluentAssertions;
using Xunit;

namespace BookStore.IntegrationTests.Controllers;

public class AssuntosControllerTests : IntegrationTestBase
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
    public async Task Create_Should_Create_Assunto_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var request = new AssuntoRequestDto { Descricao = "Ficção Científica" };

        // Act
        var response = await client.PostAsJsonAsync("/api/assuntos", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAssunto = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
        createdAssunto.Should().NotBeNull();
        createdAssunto!.Descricao.Should().Be("Ficção Científica");
        createdAssunto.CodAs.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_Should_Return_Assunto_When_Exists()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        
        // Criar assunto
        var createRequest = new AssuntoRequestDto { Descricao = "Tecnologia" };
        var createResponse = await client.PostAsJsonAsync("/api/assuntos", createRequest);
        var createdAssunto = await createResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

        // Act
        var response = await client.GetAsync($"/api/assuntos/{createdAssunto!.CodAs}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var assunto = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
        assunto.Should().NotBeNull();
        assunto!.CodAs.Should().Be(createdAssunto.CodAs);
        assunto.Descricao.Should().Be("Tecnologia");
    }

    [Fact]
    public async Task Update_Should_Update_Assunto_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        
        // Criar assunto
        var createRequest = new AssuntoRequestDto { Descricao = "Original" };
        var createResponse = await client.PostAsJsonAsync("/api/assuntos", createRequest);
        var createdAssunto = await createResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

        // Act
        var updateRequest = new AssuntoRequestDto { Descricao = "Atualizado" };
        var response = await client.PutAsJsonAsync($"/api/assuntos/{createdAssunto!.CodAs}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedAssunto = await response.Content.ReadFromJsonAsync<AssuntoResponseDto>();
        updatedAssunto.Should().NotBeNull();
        updatedAssunto!.Descricao.Should().Be("Atualizado");
    }

    [Fact]
    public async Task Delete_Should_Delete_Assunto_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        
        // Criar assunto
        var createRequest = new AssuntoRequestDto { Descricao = "To Delete" };
        var createResponse = await client.PostAsJsonAsync("/api/assuntos", createRequest);
        var createdAssunto = await createResponse.Content.ReadFromJsonAsync<AssuntoResponseDto>();

        // Act
        var response = await client.DeleteAsync($"/api/assuntos/{createdAssunto!.CodAs}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que foi deletado
        var getResponse = await client.GetAsync($"/api/assuntos/{createdAssunto.CodAs}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}



