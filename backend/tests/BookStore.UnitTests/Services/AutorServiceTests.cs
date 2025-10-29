using BookStore.Application.DTOs.Autor;
using BookStore.Application.Exceptions;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.UnitTests.Services;

public class AutorServiceTests
{
    private readonly Mock<IAutorRepository> _repositoryMock;
    private readonly AutorService _service;

    public AutorServiceTests()
    {
        _repositoryMock = new Mock<IAutorRepository>();
        _service = new AutorService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Autores()
    {
        // Arrange
        var autores = new List<Autor>
        {
            new() { CodAu = 1, Nome = "Autor 1" },
            new() { CodAu = 2, Nome = "Autor 2" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(autores);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<AutorResponseDto>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Autor_When_Exists()
    {
        // Arrange
        var autor = new Autor { CodAu = 1, Nome = "Autor Teste" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(autor);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.CodAu.Should().Be(1);
        result.Nome.Should().Be("Autor Teste");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_NotFoundException_When_Not_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Autor?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_And_Return_Autor()
    {
        // Arrange
        var request = new AutorRequestDto { Nome = "Novo Autor" };
        var createdAutor = new Autor { CodAu = 1, Nome = "Novo Autor" };
        
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Autor>())).ReturnsAsync(createdAutor);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CodAu.Should().Be(1);
        result.Nome.Should().Be("Novo Autor");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Autor>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Autor_When_Exists()
    {
        // Arrange
        var request = new AutorRequestDto { Nome = "Autor Atualizado" };
        var existingAutor = new Autor { CodAu = 1, Nome = "Autor Antigo" };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingAutor);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Autor>())).Returns(Task.FromResult(existingAutor));

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("Autor Atualizado");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Autor>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Throw_NotFoundException_When_Not_Exists()
    {
        // Arrange
        var request = new AutorRequestDto { Nome = "Autor Atualizado" };
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Autor?)null);

        // Act
        var act = async () => await _service.UpdateAsync(999, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Autor_When_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_NotFoundException_When_Not_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}

