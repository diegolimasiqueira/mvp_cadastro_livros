using BookStore.Application.DTOs.FormaCompra;
using BookStore.Application.Validators;
using FluentAssertions;
using Xunit;

namespace BookStore.UnitTests.Validators;

public class FormaCompraRequestValidatorTests
{
    private readonly FormaCompraRequestValidator _validator;

    public FormaCompraRequestValidatorTests()
    {
        _validator = new FormaCompraRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Descricao_Is_Empty()
    {
        // Arrange
        var request = new FormaCompraRequestDto { Descricao = "" };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Descricao");
    }

    [Fact]
    public void Should_Have_Error_When_Descricao_Is_Null()
    {
        // Arrange
        var request = new FormaCompraRequestDto { Descricao = null! };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Descricao");
    }

    [Fact]
    public void Should_Have_Error_When_Descricao_Exceeds_MaxLength()
    {
        // Arrange
        var request = new FormaCompraRequestDto { Descricao = new string('A', 31) };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Descricao");
    }

    [Theory]
    [InlineData("Balcão")]
    [InlineData("Online")]
    [InlineData("Autoatendimento")]
    public void Should_Not_Have_Error_When_Descricao_Is_Valid(string descricao)
    {
        // Arrange
        var request = new FormaCompraRequestDto { Descricao = descricao };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

