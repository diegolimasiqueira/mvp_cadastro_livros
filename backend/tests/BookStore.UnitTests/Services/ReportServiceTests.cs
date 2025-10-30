using BookStore.Application.DTOs.Report;
using BookStore.Application.Services;
using BookStore.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BookStore.UnitTests.Services;

public class ReportServiceTests
{
    private readonly Mock<IReportRepository> _repositoryMock;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        _repositoryMock = new Mock<IReportRepository>();
        _service = new ReportService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetBooksByAuthorReportAsync_Should_Return_All_Data()
    {
        // Arrange
        var reportData = new List<ReportData>
        {
            new()
            {
                AutorId = 1,
                AutorNome = "Robert Martin",
                LivroId = 1,
                LivroTitulo = "Clean Code",
                Editora = "Prentice Hall",
                Edicao = 1,
                AnoPublicacao = "2008",
                Assuntos = "Programação, Software",
                FormasCompra = "Físico: R$ 89.90; E-book: R$ 49.90"
            },
            new()
            {
                AutorId = 1,
                AutorNome = "Robert Martin",
                LivroId = 2,
                LivroTitulo = "Clean Architecture",
                Editora = "Prentice Hall",
                Edicao = 1,
                AnoPublicacao = "2017",
                Assuntos = "Arquitetura, Software",
                FormasCompra = "Físico: R$ 99.90"
            },
            new()
            {
                AutorId = 2,
                AutorNome = "Martin Fowler",
                LivroId = 3,
                LivroTitulo = "Refactoring",
                Editora = "Addison-Wesley",
                Edicao = 2,
                AnoPublicacao = "2018",
                Assuntos = "Programação",
                FormasCompra = "E-book: R$ 59.90"
            }
        };

        _repositoryMock.Setup(r => r.GetBooksByAuthorReportAsync()).ReturnsAsync(reportData);

        // Act
        var result = await _service.GetBooksByAuthorReportAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // 3 registros
        result.Should().AllBeOfType<BooksByAuthorReportDto>();
        
        // Verificar primeiro registro
        var firstRecord = result.First();
        firstRecord.AutorNome.Should().Be("Robert Martin");
        firstRecord.LivroTitulo.Should().Be("Clean Code");
    }

    [Fact]
    public async Task GetBooksByAuthorReportAsync_Should_Return_Empty_When_No_Data()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetBooksByAuthorReportAsync()).ReturnsAsync(new List<ReportData>());

        // Act
        var result = await _service.GetBooksByAuthorReportAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBooksByAuthorReportAsync_Should_Return_All_Authors()
    {
        // Arrange
        var reportData = new List<ReportData>
        {
            new()
            {
                AutorId = 1,
                AutorNome = "Autor A",
                LivroId = 1,
                LivroTitulo = "Livro 1",
                Editora = "Editora X",
                Edicao = 1,
                AnoPublicacao = "2020",
                Assuntos = "Teste",
                FormasCompra = "Físico: R$ 50.00"
            },
            new()
            {
                AutorId = 2,
                AutorNome = "Autor B",
                LivroId = 2,
                LivroTitulo = "Livro 2",
                Editora = "Editora Y",
                Edicao = 1,
                AnoPublicacao = "2021",
                Assuntos = "Teste",
                FormasCompra = "E-book: R$ 30.00"
            }
        };

        _repositoryMock.Setup(r => r.GetBooksByAuthorReportAsync()).ReturnsAsync(reportData);

        // Act
        var result = await _service.GetBooksByAuthorReportAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(r => r.AutorNome == "Autor A");
        result.Should().Contain(r => r.AutorNome == "Autor B");
    }

    [Fact]
    public async Task GetBooksByAuthorReportAsync_Should_Map_All_Fields_Correctly()
    {
        // Arrange
        var reportData = new List<ReportData>
        {
            new()
            {
                AutorId = 1,
                AutorNome = "Test Author",
                LivroId = 1,
                LivroTitulo = "Test Book",
                Editora = "Test Publisher",
                Edicao = 2,
                AnoPublicacao = "2022",
                Assuntos = "Subject 1, Subject 2",
                FormasCompra = "Físico: R$ 45.00; E-book: R$ 25.00"
            }
        };

        _repositoryMock.Setup(r => r.GetBooksByAuthorReportAsync()).ReturnsAsync(reportData);

        // Act
        var result = await _service.GetBooksByAuthorReportAsync();

        // Assert
        var record = result.First();
        record.AutorId.Should().Be(1);
        record.AutorNome.Should().Be("Test Author");
        record.LivroId.Should().Be(1);
        record.LivroTitulo.Should().Be("Test Book");
        record.Editora.Should().Be("Test Publisher");
        record.Edicao.Should().Be(2);
        record.AnoPublicacao.Should().Be("2022");
        record.Assuntos.Should().Be("Subject 1, Subject 2");
        record.FormasCompra.Should().Be("Físico: R$ 45.00; E-book: R$ 25.00");
    }
}
