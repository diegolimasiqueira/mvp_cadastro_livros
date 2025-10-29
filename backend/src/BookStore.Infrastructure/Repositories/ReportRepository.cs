using BookStore.Domain.Entities.Views;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de relatórios
/// </summary>
public class ReportRepository : IReportRepository
{
    private readonly BookStoreDbContext _context;

    public ReportRepository(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ReportData>> GetBooksByAuthorReportAsync()
    {
        // Consulta a VIEW vw_livros_por_autor criada no banco via migration
        var viewData = await _context.VwLivrosPorAutor
            .AsNoTracking()
            .ToListAsync();

        // Mapear os dados da VIEW para o modelo ReportData
        var result = viewData.Select(v => new ReportData
        {
            AutorId = v.AutorCodigo,
            AutorNome = v.AutorNome,
            LivroId = v.LivroCodigo,
            LivroTitulo = v.LivroTitulo,
            Editora = v.Editora,
            Edicao = v.Edicao,
            AnoPublicacao = v.AnoPublicacao,
            Assuntos = v.Assuntos ?? string.Empty,
            FormasCompra = v.FormasCompra ?? string.Empty
        });

        return result;
    }
}

