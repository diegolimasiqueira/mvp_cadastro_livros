namespace BookStore.Domain.Interfaces;

/// <summary>
/// Interface para repositório de relatórios com consultas especializadas
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Obtém dados do relatório de livros agrupados por autor
    /// Este método deve usar uma VIEW no banco de dados
    /// </summary>
    Task<IEnumerable<ReportData>> GetBooksByAuthorReportAsync();
}

/// <summary>
/// Dados brutos do relatório de livros por autor vindos do repositório
/// </summary>
public class ReportData
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

