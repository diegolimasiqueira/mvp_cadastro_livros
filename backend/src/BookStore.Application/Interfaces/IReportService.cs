using BookStore.Application.DTOs.Report;

namespace BookStore.Application.Interfaces;

/// <summary>
/// Interface para serviço de relatórios
/// </summary>
public interface IReportService
{
    Task<IEnumerable<BooksByAuthorReportDto>> GetBooksByAuthorReportAsync();
}



