namespace BookStore.Application.DTOs.Report;

/// <summary>
/// DTO para relatório de livros por autor
/// </summary>
public class BooksByAuthorReportDto
{
    public int AutorId { get; set; }
    public string AutorNome { get; set; } = string.Empty;
    public int LivroId { get; set; }
    public string LivroTitulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;
    public string Assuntos { get; set; } = string.Empty;
    public string FormasCompra { get; set; } = string.Empty;
}



