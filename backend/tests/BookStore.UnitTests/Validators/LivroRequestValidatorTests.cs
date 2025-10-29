using BookStore.Application.DTOs.Livro;
using BookStore.Application.Validators;
using FluentAssertions;
using Xunit;

namespace BookStore.UnitTests.Validators;

public class LivroRequestValidatorTests
{
    private readonly LivroRequestValidator _validator;

    public LivroRequestValidatorTests()
    {
        _validator = new LivroRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Titulo_Is_Empty()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "",
            Editora = "Editora Teste",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>()
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Titulo");
    }

    [Fact]
    public void Should_Have_Error_When_Editora_Is_Empty()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Título Teste",
            Editora = "",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>()
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Editora");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_Edicao_Is_Invalid(int edicao)
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Título Teste",
            Editora = "Editora Teste",
            Edicao = edicao,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>()
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Edicao");
    }

    [Fact]
    public void Should_Have_Error_When_AutorIds_Is_Empty()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Título Teste",
            Editora = "Editora Teste",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int>(),
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>()
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AutoresIds");
    }

    [Fact]
    public void Should_Have_Error_When_AssuntoIds_Is_Empty()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Título Teste",
            Editora = "Editora Teste",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int>(),
            Precos = new List<LivroPrecoRequestDto>()
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AssuntosIds");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Clean Code",
            Editora = "Prentice Hall",
            Edicao = 1,
            AnoPublicacao = "2008",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>
            {
                new() { FormaCompraId = 1, Valor = 50.00m }
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}

