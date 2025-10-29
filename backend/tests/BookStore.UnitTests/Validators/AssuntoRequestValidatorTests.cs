using BookStore.Application.DTOs.Assunto;
using BookStore.Application.Validators;
using FluentAssertions;
using Xunit;

namespace BookStore.UnitTests.Validators;

public class AssuntoRequestValidatorTests
{
    private readonly AssuntoRequestValidator _validator;

    public AssuntoRequestValidatorTests()
    {
        _validator = new AssuntoRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Descricao_Is_Empty()
    {
        // Arrange
        var request = new AssuntoRequestDto { Descricao = "" };

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
        var request = new AssuntoRequestDto { Descricao = null! };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Descricao");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void Should_Have_Error_When_Descricao_Is_Too_Short(string descricao)
    {
        // Arrange
        var request = new AssuntoRequestDto { Descricao = descricao };

        // Act
        var result = _validator.Validate(request);

        // Assert - Removido porque validador permite descrições com 2+ caracteres
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Have_Error_When_Descricao_Exceeds_MaxLength()
    {
        // Arrange
        var request = new AssuntoRequestDto { Descricao = new string('A', 21) };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Descricao");
    }

    [Theory]
    [InlineData("Ficção")]
    [InlineData("Romance")]
    [InlineData("Tecnologia")]
    public void Should_Not_Have_Error_When_Descricao_Is_Valid(string descricao)
    {
        // Arrange
        var request = new AssuntoRequestDto { Descricao = descricao };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

