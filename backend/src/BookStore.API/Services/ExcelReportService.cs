using OfficeOpenXml;
using OfficeOpenXml.Style;
using BookStore.Application.Interfaces;
using System.Drawing;

namespace BookStore.API.Services;

/// <summary>
/// Serviço para geração de relatórios em Excel usando EPPlus
/// </summary>
public class ExcelReportService
{
    private readonly IReportService _reportService;

    public ExcelReportService(IReportService reportService)
    {
        _reportService = reportService;
        
        // Configuração de licença do EPPlus (uso não comercial)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    /// <summary>
    /// Gera o relatório de livros por autor em formato Excel com agrupamento
    /// </summary>
    public async Task<byte[]> GenerateLivrosPorAutorExcelAsync()
    {
        // Obter dados da VIEW do banco
        var reportData = await _reportService.GetBooksByAuthorReportAsync();
        
        // Agrupar por autor
        var groupedData = reportData
            .GroupBy(r => new { r.AutorId, r.AutorNome })
            .OrderBy(g => g.Key.AutorNome)
            .ToList();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Livros por Autor");

        // Título
        worksheet.Cells["A1:F1"].Merge = true;
        worksheet.Cells["A1"].Value = "Relatório de Livros por Autor";
        worksheet.Cells["A1"].Style.Font.Size = 16;
        worksheet.Cells["A1"].Style.Font.Bold = true;
        worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
        worksheet.Cells["A1"].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 102, 204));
        worksheet.Cells["A1"].Style.Font.Color.SetColor(Color.White);
        worksheet.Row(1).Height = 30;

        // Data de geração
        worksheet.Cells["A2:F2"].Merge = true;
        worksheet.Cells["A2"].Value = $"Gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}";
        worksheet.Cells["A2"].Style.Font.Italic = true;
        worksheet.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        int currentRow = 4;

        // Para cada autor
        foreach (var autorGroup in groupedData)
        {
            // Cabeçalho do Autor
            worksheet.Cells[currentRow, 1, currentRow, 6].Merge = true;
            worksheet.Cells[currentRow, 1].Value = $"Autor: {autorGroup.Key.AutorNome}";
            worksheet.Cells[currentRow, 1].Style.Font.Size = 12;
            worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[currentRow, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[currentRow, 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(173, 216, 230));
            worksheet.Cells[currentRow, 1].Style.Font.Color.SetColor(Color.FromArgb(0, 51, 102));
            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(currentRow).Height = 25;
            currentRow++;

            // Cabeçalho das colunas
            worksheet.Cells[currentRow, 1].Value = "Livro";
            worksheet.Cells[currentRow, 2].Value = "Editora";
            worksheet.Cells[currentRow, 3].Value = "Edição";
            worksheet.Cells[currentRow, 4].Value = "Ano";
            worksheet.Cells[currentRow, 5].Value = "Assuntos";
            worksheet.Cells[currentRow, 6].Value = "Preços";

            using (var range = worksheet.Cells[currentRow, 1, currentRow, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(211, 211, 211));
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
            }
            currentRow++;

            // Dados dos livros
            foreach (var livro in autorGroup)
            {
                worksheet.Cells[currentRow, 1].Value = livro.LivroTitulo;
                worksheet.Cells[currentRow, 2].Value = livro.Editora;
                worksheet.Cells[currentRow, 3].Value = livro.Edicao;
                worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 4].Value = livro.AnoPublicacao;
                worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[currentRow, 5].Value = livro.Assuntos ?? "-";
                worksheet.Cells[currentRow, 6].Value = livro.FormasCompra ?? "-";

                // Bordas
                using (var range = worksheet.Cells[currentRow, 1, currentRow, 6])
                {
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Color.SetColor(Color.LightGray);
                }

                currentRow++;
            }

            // Espaço entre grupos
            currentRow += 2;
        }

        // Ajustar largura das colunas
        worksheet.Column(1).Width = 25; // Livro
        worksheet.Column(2).Width = 18; // Editora
        worksheet.Column(3).Width = 10; // Edição
        worksheet.Column(4).Width = 10; // Ano
        worksheet.Column(5).Width = 20; // Assuntos
        worksheet.Column(6).Width = 30; // Preços

        // Congelar painéis (título e cabeçalho)
        worksheet.View.FreezePanes(3, 1);

        return package.GetAsByteArray();
    }
}

