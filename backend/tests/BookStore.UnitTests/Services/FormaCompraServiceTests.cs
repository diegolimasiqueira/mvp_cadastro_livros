using BookStore.Application.DTOs.FormaCompra;
using BookStore.Application.Exceptions;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.UnitTests.Services;

public class FormaCompraServiceTests
{
    private readonly Mock<IFormaCompraRepository> _repositoryMock;
    private readonly FormaCompraService _service;

    public FormaCompraServiceTests()
    {
        _repositoryMock = new Mock<IFormaCompraRepository>();
        _service = new FormaCompraService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_FormasCompra()
    {
        // Arrange
        var formasCompra = new List<FormaCompra>
        {
            new() { CodFc = 1, Descricao = "Balcão" },
            new() { CodFc = 2, Descricao = "Online" }
        };
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(formasCompra);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<FormaCompraResponseDto>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_FormaCompra_When_Exists()
    {
        // Arrange
        var formaCompra = new FormaCompra { CodFc = 1, Descricao = "Balcão" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(formaCompra);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.CodFc.Should().Be(1);
        result.Descricao.Should().Be("Balcão");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_NotFoundException_When_Not_Exists()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FormaCompra?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_And_Return_FormaCompra()
    {
        // Arrange
        var request = new FormaCompraRequestDto { Descricao = "Autoatendimento" };
        var created = new FormaCompra { CodFc = 1, Descricao = "Autoatendimento" };
        
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<FormaCompra>())).ReturnsAsync(created);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CodFc.Should().Be(1);
        result.Descricao.Should().Be("Autoatendimento");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<FormaCompra>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_FormaCompra_When_Exists()
    {
        // Arrange
        var request = new FormaCompraRequestDto { Descricao = "Atualizado" };
        var existing = new FormaCompra { CodFc = 1, Descricao = "Antigo" };
        
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<FormaCompra>())).Returns(Task.FromResult(existing));

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result.Descricao.Should().Be("Atualizado");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FormaCompra>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_FormaCompra_When_Exists()
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



