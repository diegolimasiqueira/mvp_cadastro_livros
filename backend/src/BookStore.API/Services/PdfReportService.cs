using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using BookStore.Application.Interfaces;

namespace BookStore.API.Services;

/// <summary>
/// Serviço para geração de relatórios em PDF usando QuestPDF
/// </summary>
public class PdfReportService
{
    private readonly IReportService _reportService;

    public PdfReportService(IReportService reportService)
    {
        _reportService = reportService;
        
        // Configuração de licença do QuestPDF (uso não comercial/community)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Gera o relatório de livros por autor em formato PDF com agrupamento
    /// </summary>
    public async Task<byte[]> GenerateLivrosPorAutorPdfAsync()
    {
        // Obter dados da VIEW do banco
        var reportData = await _reportService.GetBooksByAuthorReportAsync();
        
        // Agrupar por autor
        var groupedData = reportData
            .GroupBy(r => new { r.AutorId, r.AutorNome })
            .OrderBy(g => g.Key.AutorNome)
            .ToList();

        // Gerar PDF
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // Cabeçalho
                page.Header()
                    .Height(60)
                    .Background(Colors.Blue.Darken1)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("Relatório de Livros por Autor")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.White);

                // Conteúdo
                page.Content()
                    .PaddingVertical(10)
                    .Column(column =>
                    {
                        // Para cada autor
                        foreach (var autorGroup in groupedData)
                        {
                            // Nome do Autor (Cabeçalho do grupo)
                            column.Item()
                                .PaddingTop(10)
                                .Background(Colors.Blue.Lighten3)
                                .Padding(10)
                                .Text($"Autor: {autorGroup.Key.AutorNome}")
                                .FontSize(14)
                                .Bold()
                                .FontColor(Colors.Blue.Darken4);

                            // Tabela de livros do autor
                            column.Item()
                                .PaddingTop(5)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); // Livro
                                        columns.RelativeColumn(2); // Editora
                                        columns.RelativeColumn(1); // Edição
                                        columns.RelativeColumn(1); // Ano
                                        columns.RelativeColumn(2); // Assuntos
                                        columns.RelativeColumn(3); // Preços
                                    });

                                    // Cabeçalho da tabela
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Livro").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Editora").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("Ed.").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text("Ano").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Assuntos").Bold();
                                        header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Preços").Bold();
                                    });

                                    // Linhas de dados
                                    foreach (var livro in autorGroup)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(livro.LivroTitulo);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(livro.Editora);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(livro.Edicao.ToString());
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignCenter().Text(livro.AnoPublicacao);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(livro.Assuntos ?? "-").FontSize(8);
                                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(livro.FormasCompra ?? "-").FontSize(8);
                                    }
                                });

                            // Espaçamento entre grupos
                            column.Item().PaddingBottom(10);
                        }
                    });

                // Rodapé
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Gerado em: ");
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).Bold();
                        text.Span(" - Página ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                        text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    });
            });
        })
        .GeneratePdf();
    }
}
