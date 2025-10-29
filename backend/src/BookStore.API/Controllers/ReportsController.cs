using BookStore.API.Services;
using BookStore.Application.DTOs.Report;
using BookStore.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

/// <summary>
/// Controller para relatórios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly PdfReportService _pdfReportService;
    private readonly ExcelReportService _excelReportService;

    public ReportsController(
        IReportService reportService, 
        PdfReportService pdfReportService,
        ExcelReportService excelReportService)
    {
        _reportService = reportService;
        _pdfReportService = pdfReportService;
        _excelReportService = excelReportService;
    }

    /// <summary>
    /// Obtém relatório de livros agrupados por autor (JSON)
    /// </summary>
    /// <remarks>
    /// Este relatório mostra todos os livros com seus autores, assuntos e preços por forma de compra.
    /// Os dados são agrupados por autor para facilitar a visualização.
    /// </remarks>
    /// <returns>Dados do relatório em formato JSON</returns>
    [HttpGet("books-by-author")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IEnumerable<BooksByAuthorReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BooksByAuthorReportDto>>> GetBooksByAuthor()
    {
        var report = await _reportService.GetBooksByAuthorReportAsync();
        return Ok(report);
    }

    /// <summary>
    /// Gera relatório de livros por autor em formato PDF
    /// </summary>
    /// <remarks>
    /// Este endpoint gera um relatório PDF profissional utilizando QuestPDF.
    /// O relatório é gerado a partir de uma VIEW do banco de dados e mostra todos os livros agrupados por autor,
    /// incluindo informações de editora, edição, ano de publicação, assuntos e preços por forma de compra.
    /// 
    /// Tecnologias utilizadas:
    /// - VIEW no PostgreSQL (vw_livros_por_autor)
    /// - QuestPDF (componente moderno de geração de PDFs)
    /// - Agrupamento visual por autor com cabeçalhos destacados
    /// </remarks>
    /// <returns>Arquivo PDF do relatório</returns>
    [HttpGet("books-by-author/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBooksByAuthorPdf()
    {
        var pdfBytes = await _pdfReportService.GenerateLivrosPorAutorPdfAsync();
        var fileName = $"Relatorio_Livros_Por_Autor_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Gera relatório de livros por autor em formato Excel
    /// </summary>
    /// <remarks>
    /// Este endpoint gera um relatório Excel profissional utilizando EPPlus.
    /// O relatório é gerado a partir de uma VIEW do banco de dados e mostra todos os livros agrupados por autor,
    /// incluindo informações de editora, edição, ano de publicação, assuntos e preços por forma de compra.
    /// 
    /// Tecnologias utilizadas:
    /// - VIEW no PostgreSQL (vw_livros_por_autor)
    /// - EPPlus (componente de geração de planilhas Excel)
    /// - Formatação com cores, bordas e agrupamento por autor
    /// </remarks>
    /// <returns>Arquivo Excel (.xlsx) do relatório</returns>
    [HttpGet("books-by-author/excel")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBooksByAuthorExcel()
    {
        var excelBytes = await _excelReportService.GenerateLivrosPorAutorExcelAsync();
        var fileName = $"Relatorio_Livros_Por_Autor_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}



