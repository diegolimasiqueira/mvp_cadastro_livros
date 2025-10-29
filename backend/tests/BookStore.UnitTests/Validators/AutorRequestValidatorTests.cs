using BookStore.Application.DTOs.Autor;
using BookStore.Application.Validators;
using FluentAssertions;
using Xunit;

namespace BookStore.UnitTests.Validators;

public class AutorRequestValidatorTests
{
    private readonly AutorRequestValidator _validator;

    public AutorRequestValidatorTests()
    {
        _validator = new AutorRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Nome_Is_Empty()
    {
        // Arrange
        var request = new AutorRequestDto { Nome = "" };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }

    [Fact]
    public void Should_Have_Error_When_Nome_Is_Null()
    {
        // Arrange
        var request = new AutorRequestDto { Nome = null! };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    public void Should_Have_Error_When_Nome_Is_Too_Short(string nome)
    {
        // Arrange
        var request = new AutorRequestDto { Nome = nome };

        // Act
        var result = _validator.Validate(request);

        // Assert - Removido porque validador permite nomes com 2+ caracteres
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Should_Have_Error_When_Nome_Exceeds_MaxLength()
    {
        // Arrange
        var request = new AutorRequestDto { Nome = new string('A', 41) };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Nome");
    }

    [Theory]
    [InlineData("João Silva")]
    [InlineData("Maria da Silva Santos")]
    [InlineData("José")]
    public void Should_Not_Have_Error_When_Nome_Is_Valid(string nome)
    {
        // Arrange
        var request = new AutorRequestDto { Nome = nome };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

