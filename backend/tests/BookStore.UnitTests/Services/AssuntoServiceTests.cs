using BookStore.Application.DTOs.Assunto;
using BookStore.Application.Exceptions;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.UnitTests.Services;

public class AssuntoServiceTests
{
    private readonly Mock<IAssuntoRepository> _repositoryMock;
    private readonly AssuntoService _service;

    public AssuntoServiceTests()
    {
        _repositoryMock = new Mock<IAssuntoRepository>();
        _service = new AssuntoService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Assuntos()
    {
        // Arrange
        var assuntos = new List<Assunto>
        {
            new() { CodAs = 1, Descricao = "Ficção" },
            new() { CodAs = 2, Descricao = "Romance" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(assuntos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<AssuntoResponseDto>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Assunto_When_Exists()
    {
        // Arrange
        var assunto = new Assunto { CodAs = 1, Descricao = "Tecnologia" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assunto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.CodAs.Should().Be(1);
        result.Descricao.Should().Be("Tecnologia");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_NotFoundException_When_Not_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Assunto?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_And_Return_Assunto()
    {
        // Arrange
        var request = new AssuntoRequestDto { Descricao = "Novo Assunto" };
        var createdAssunto = new Assunto { CodAs = 1, Descricao = "Novo Assunto" };
        
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Assunto>())).ReturnsAsync(createdAssunto);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CodAs.Should().Be(1);
        result.Descricao.Should().Be("Novo Assunto");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Assunto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Assunto_When_Exists()
    {
        // Arrange
        var request = new AssuntoRequestDto { Descricao = "Assunto Atualizado" };
        var existingAssunto = new Assunto { CodAs = 1, Descricao = "Assunto Antigo" };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingAssunto);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Assunto>())).Returns(Task.FromResult(existingAssunto));

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Descricao.Should().Be("Assunto Atualizado");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Assunto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Assunto_When_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _repositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }
}

