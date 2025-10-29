using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookStore.Application.DTOs.Autor;
using BookStore.Application.DTOs.Auth;
using BookStore.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.IntegrationTests.Controllers;

public class AutoresControllerTests : IntegrationTestBase
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
    public async Task GetAll_Should_Return_Empty_List_Initially()
    {
        // Arrange
        var client = await GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/autores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var autores = await response.Content.ReadFromJsonAsync<List<AutorResponseDto>>();
        autores.Should().NotBeNull();
        autores.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_Should_Create_Autor_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var request = new AutorRequestDto { Nome = "Test Author" };

        // Act
        var response = await client.PostAsJsonAsync("/api/autores", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdAutor = await response.Content.ReadFromJsonAsync<AutorResponseDto>();
        createdAutor.Should().NotBeNull();
        createdAutor!.Nome.Should().Be("Test Author");
        createdAutor.CodAu.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetById_Should_Return_Autor_When_Exists()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        
        // Criar autor
        var createRequest = new AutorRequestDto { Nome = "Author for GetById" };
        var createResponse = await client.PostAsJsonAsync("/api/autores", createRequest);
        var createdAutor = await createResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

        // Act
        var response = await client.GetAsync($"/api/autores/{createdAutor!.CodAu}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var autor = await response.Content.ReadFromJsonAsync<AutorResponseDto>();
        autor.Should().NotBeNull();
        autor!.CodAu.Should().Be(createdAutor.CodAu);
        autor.Nome.Should().Be("Author for GetById");
    }

    [Fact]
    public async Task GetById_Should_Return_NotFound_When_Not_Exists()
    {
        // Arrange
        var client = await GetAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/autores/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_Should_Update_Autor_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        
        // Criar autor
        var createRequest = new AutorRequestDto { Nome = "Original Name" };
        var createResponse = await client.PostAsJsonAsync("/api/autores", createRequest);
        var createdAutor = await createResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

        // Act
        var updateRequest = new AutorRequestDto { Nome = "Updated Name" };
        var response = await client.PutAsJsonAsync($"/api/autores/{createdAutor!.CodAu}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedAutor = await response.Content.ReadFromJsonAsync<AutorResponseDto>();
        updatedAutor.Should().NotBeNull();
        updatedAutor!.Nome.Should().Be("Updated Name");
    }

    [Fact]
    public async Task Delete_Should_Delete_Autor_Successfully()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        
        // Criar autor
        var createRequest = new AutorRequestDto { Nome = "Author to Delete" };
        var createResponse = await client.PostAsJsonAsync("/api/autores", createRequest);
        var createdAutor = await createResponse.Content.ReadFromJsonAsync<AutorResponseDto>();

        // Act
        var response = await client.DeleteAsync($"/api/autores/{createdAutor!.CodAu}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verificar que foi deletado
        var getResponse = await client.GetAsync($"/api/autores/{createdAutor.CodAu}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Invalid_Data()
    {
        // Arrange
        var client = await GetAuthenticatedClient();
        var request = new AutorRequestDto { Nome = "" }; // Nome vazio

        // Act
        var response = await client.PostAsJsonAsync("/api/autores", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}



