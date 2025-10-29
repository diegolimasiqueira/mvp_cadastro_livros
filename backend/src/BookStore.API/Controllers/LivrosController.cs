using BookStore.Application.DTOs.Common;
using BookStore.Application.DTOs.Livro;
using BookStore.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

/// <summary>
/// Controller para gerenciamento de livros
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _livroService;
    private readonly IValidator<LivroRequestDto> _validator;
    private readonly ILogger<LivrosController> _logger;

    public LivrosController(ILivroService livroService, IValidator<LivroRequestDto> validator, ILogger<LivrosController> logger)
    {
        _livroService = livroService;
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os livros com seus relacionamentos
    /// </summary>
    /// <returns>Lista de livros</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LivroResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LivroResponseDto>>> GetAll()
    {
        var livros = await _livroService.GetAllAsync();
        return Ok(livros);
    }

    /// <summary>
    /// Obtém livros paginados com seus relacionamentos
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    /// <returns>Resposta paginada com livros</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<LivroResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<LivroResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _livroService.GetPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um livro por ID com todos os seus relacionamentos
    /// </summary>
    /// <param name="id">ID do livro</param>
    /// <returns>Dados completos do livro</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LivroResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponseDto>> GetById(int id)
    {
        var livro = await _livroService.GetByIdAsync(id);
        return Ok(livro);
    }

    /// <summary>
    /// Cria um novo livro com seus relacionamentos
    /// </summary>
    /// <param name="request">Dados do livro incluindo autores, assuntos e preços</param>
    /// <returns>Livro criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(LivroResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LivroResponseDto>> Create([FromBody] LivroRequestDto request)
    {
        _logger.LogInformation("📘 Iniciando criação de novo livro | Título: {Titulo} | Editora: {Editora} | Ano: {Ano}",
            request.Titulo, request.Editora, request.AnoPublicacao);

        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("⚠️ Falha na validação ao criar livro | Título: {Titulo} | Erros: {@ValidationErrors}",
                request.Titulo, validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        var livro = await _livroService.CreateAsync(request);
        
        _logger.LogInformation("✅ Livro criado com sucesso! | ID: {LivroId} | Título: {Titulo} | Autores: {AutoresCount} | Assuntos: {AssuntosCount} | Preços: {PrecosCount} | Usuario: {Usuario}",
            livro.CodI, livro.Titulo, request.AutoresIds?.Count ?? 0, request.AssuntosIds?.Count ?? 0, request.Precos?.Count ?? 0, User.Identity?.Name ?? "Anônimo");
        
        return CreatedAtAction(nameof(GetById), new { id = livro.CodI }, livro);
    }

    /// <summary>
    /// Atualiza um livro existente e seus relacionamentos
    /// </summary>
    /// <param name="id">ID do livro</param>
    /// <param name="request">Novos dados do livro</param>
    /// <returns>Livro atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(LivroResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponseDto>> Update(int id, [FromBody] LivroRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var livro = await _livroService.UpdateAsync(id, request);
        return Ok(livro);
    }

    /// <summary>
    /// Deleta um livro e seus relacionamentos
    /// </summary>
    /// <param name="id">ID do livro</param>
    /// <returns>Confirmação de deleção</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _livroService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Busca livros por título
    /// </summary>
    /// <param name="title">Título ou parte do título</param>
    /// <returns>Lista de livros encontrados</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<LivroResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LivroResponseDto>>> Search([FromQuery] string title)
    {
        var livros = await _livroService.SearchByTitleAsync(title);
        return Ok(livros);
    }

    /// <summary>
    /// Obtém livros de um autor específico
    /// </summary>
    /// <param name="authorId">ID do autor</param>
    /// <returns>Lista de livros do autor</returns>
    [HttpGet("by-author/{authorId}")]
    [ProducesResponseType(typeof(IEnumerable<LivroResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LivroResponseDto>>> GetByAuthor(int authorId)
    {
        var livros = await _livroService.GetByAuthorAsync(authorId);
        return Ok(livros);
    }

    /// <summary>
    /// Obtém livros de um assunto específico
    /// </summary>
    /// <param name="subjectId">ID do assunto</param>
    /// <returns>Lista de livros do assunto</returns>
    [HttpGet("by-subject/{subjectId}")]
    [ProducesResponseType(typeof(IEnumerable<LivroResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LivroResponseDto>>> GetBySubject(int subjectId)
    {
        var livros = await _livroService.GetBySubjectAsync(subjectId);
        return Ok(livros);
    }
}



