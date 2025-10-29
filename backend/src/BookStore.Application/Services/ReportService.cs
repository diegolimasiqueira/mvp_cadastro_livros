using BookStore.Application.DTOs.Report;
using BookStore.Application.Interfaces;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services;

/// <summary>
/// Serviço de negócio para Relatórios
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;

    public ReportService(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<IEnumerable<BooksByAuthorReportDto>> GetBooksByAuthorReportAsync()
    {
        var reportData = await _reportRepository.GetBooksByAuthorReportAsync();
        
        return reportData.Select(r => new DTOs.Report.BooksByAuthorReportDto
        {
            AutorId = r.AutorId,
            AutorNome = r.AutorNome,
            LivroId = r.LivroId,
            LivroTitulo = r.LivroTitulo,
            Editora = r.Editora,
            Edicao = r.Edicao,
            AnoPublicacao = r.AnoPublicacao,
            Assuntos = r.Assuntos,
            FormasCompra = r.FormasCompra
        });
    }
}

