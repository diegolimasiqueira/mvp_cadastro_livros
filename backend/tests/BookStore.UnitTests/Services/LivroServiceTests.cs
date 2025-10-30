using BookStore.Application.DTOs.Livro;
using BookStore.Application.Exceptions;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.UnitTests.Services;

public class LivroServiceTests
{
    private readonly Mock<ILivroRepository> _livroRepositoryMock;
    private readonly Mock<IAutorRepository> _autorRepositoryMock;
    private readonly Mock<IAssuntoRepository> _assuntoRepositoryMock;
    private readonly Mock<IFormaCompraRepository> _formaCompraRepositoryMock;
    private readonly LivroService _service;

    public LivroServiceTests()
    {
        _livroRepositoryMock = new Mock<ILivroRepository>();
        _autorRepositoryMock = new Mock<IAutorRepository>();
        _assuntoRepositoryMock = new Mock<IAssuntoRepository>();
        _formaCompraRepositoryMock = new Mock<IFormaCompraRepository>();
        
        _service = new LivroService(
            _livroRepositoryMock.Object,
            _autorRepositoryMock.Object,
            _assuntoRepositoryMock.Object,
            _formaCompraRepositoryMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_Livros()
    {
        // Arrange
        var livros = new List<Livro>
        {
            new() 
            { 
                CodI = 1, 
                Titulo = "Livro 1", 
                Editora = "Editora A",
                Edicao = 1,
                AnoPublicacao = "2023"
            },
            new() 
            { 
                CodI = 2, 
                Titulo = "Livro 2", 
                Editora = "Editora B",
                Edicao = 2,
                AnoPublicacao = "2024"
            }
        };
        _livroRepositoryMock.Setup(r => r.GetWithAuthorsAndSubjectsAsync()).ReturnsAsync(livros);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<LivroResponseDto>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Livro_When_Exists()
    {
        // Arrange
        var autor = new Autor { CodAu = 1, Nome = "Robert Martin" };
        var assunto = new Assunto { CodAs = 1, Descricao = "Programação" };
        var formaCompra = new FormaCompra { CodFc = 1, Descricao = "Físico" };
        
        var livro = new Livro 
        { 
            CodI = 1, 
            Titulo = "Clean Code", 
            Editora = "Prentice Hall",
            Edicao = 1,
            AnoPublicacao = "2008"
        };
        
        // Adicionar relacionamentos
        livro.LivroAutores.Add(new LivroAutor { Livro_CodI = 1, Autor_CodAu = 1, Autor = autor });
        livro.LivroAssuntos.Add(new LivroAssunto { Livro_CodI = 1, Assunto_CodAs = 1, Assunto = assunto });
        livro.LivroPrecos.Add(new LivroPreco 
        { 
            CodLp = 1, 
            Livro_CodI = 1,
            FormaCompra_CodFc = 1, 
            Valor = 89.90M, 
            FormaCompra = formaCompra 
        });
        
        _livroRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(livro);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.CodI.Should().Be(1);
        result.Titulo.Should().Be("Clean Code");
        result.Autores.Should().HaveCount(1);
        result.Assuntos.Should().HaveCount(1);
        result.Precos.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Throw_NotFoundException_When_Not_Exists()
    {
        // Arrange
        _livroRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(999)).ReturnsAsync((Livro?)null);

        // Act
        var act = async () => await _service.GetByIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Book with id 999 was not found");
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Livro_Successfully()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Novo Livro",
            Editora = "Nova Editora",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>
            {
                new() { FormaCompraId = 1, Valor = 49.90M }
            }
        };

        // Mockar ExistsAsync para validações
        _autorRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _assuntoRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _formaCompraRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        
        var autor = new Autor { CodAu = 1, Nome = "Autor Teste" };
        var assunto = new Assunto { CodAs = 1, Descricao = "Assunto Teste" };
        var formaCompra = new FormaCompra { CodFc = 1, Descricao = "Físico" };
        
        _autorRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(autor);
        _assuntoRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assunto);
        _formaCompraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(formaCompra);
        
        var createdLivro = new Livro 
        { 
            CodI = 1, 
            Titulo = request.Titulo,
            Editora = request.Editora,
            Edicao = request.Edicao,
            AnoPublicacao = request.AnoPublicacao
        };
        
        createdLivro.LivroAutores.Add(new LivroAutor { Livro_CodI = 1, Autor_CodAu = 1, Autor = autor });
        createdLivro.LivroAssuntos.Add(new LivroAssunto { Livro_CodI = 1, Assunto_CodAs = 1, Assunto = assunto });
        createdLivro.LivroPrecos.Add(new LivroPreco 
        { 
            CodLp = 1,
            Livro_CodI = 1,
            Valor = 49.90M, 
            FormaCompra = formaCompra,
            FormaCompra_CodFc = 1
        });
        
        _livroRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Livro>())).ReturnsAsync(createdLivro);
        _livroRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(createdLivro);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Titulo.Should().Be("Novo Livro");
        result.Autores.Should().HaveCount(1);
        result.Assuntos.Should().HaveCount(1);
        result.Precos.Should().HaveCount(1);
        _livroRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Livro>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BusinessException_When_Autor_Not_Exists()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Novo Livro",
            Editora = "Nova Editora",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 999 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>()
        };

        _autorRepositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Author with id 999 does not exist");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BusinessException_When_Assunto_Not_Exists()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Novo Livro",
            Editora = "Nova Editora",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 999 },
            Precos = new List<LivroPrecoRequestDto>()
        };

        _autorRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _assuntoRepositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Subject with id 999 does not exist");
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_BusinessException_When_FormaCompra_Not_Exists()
    {
        // Arrange
        var request = new LivroRequestDto
        {
            Titulo = "Novo Livro",
            Editora = "Nova Editora",
            Edicao = 1,
            AnoPublicacao = "2024",
            AutoresIds = new List<int> { 1 },
            AssuntosIds = new List<int> { 1 },
            Precos = new List<LivroPrecoRequestDto>
            {
                new() { FormaCompraId = 999, Valor = 49.90M }
            }
        };

        _autorRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _assuntoRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _formaCompraRepositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("Purchase method with id 999 does not exist");
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Livro_Successfully()
    {
        // Arrange
        _livroRepositoryMock.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _livroRepositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _livroRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Throw_NotFoundException_When_Livro_Not_Exists()
    {
        // Arrange
        _livroRepositoryMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act
        var act = async () => await _service.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Book with id 999 was not found");
    }
}
