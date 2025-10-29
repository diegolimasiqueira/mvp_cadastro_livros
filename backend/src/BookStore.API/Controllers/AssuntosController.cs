using BookStore.Application.DTOs.Assunto;
using BookStore.Application.DTOs.Common;
using BookStore.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

/// <summary>
/// Controller para gerenciamento de assuntos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class AssuntosController : ControllerBase
{
    private readonly IAssuntoService _assuntoService;
    private readonly IValidator<AssuntoRequestDto> _validator;

    public AssuntosController(IAssuntoService assuntoService, IValidator<AssuntoRequestDto> validator)
    {
        _assuntoService = assuntoService;
        _validator = validator;
    }

    /// <summary>
    /// Obtém todos os assuntos
    /// </summary>
    /// <returns>Lista de assuntos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AssuntoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AssuntoResponseDto>>> GetAll()
    {
        var assuntos = await _assuntoService.GetAllAsync();
        return Ok(assuntos);
    }

    /// <summary>
    /// Obtém assuntos paginados
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    /// <returns>Resposta paginada com assuntos</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<AssuntoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AssuntoResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _assuntoService.GetPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um assunto por ID
    /// </summary>
    /// <param name="id">ID do assunto</param>
    /// <returns>Dados do assunto</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AssuntoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssuntoResponseDto>> GetById(int id)
    {
        var assunto = await _assuntoService.GetByIdAsync(id);
        return Ok(assunto);
    }

    /// <summary>
    /// Cria um novo assunto
    /// </summary>
    /// <param name="request">Dados do assunto</param>
    /// <returns>Assunto criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AssuntoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AssuntoResponseDto>> Create([FromBody] AssuntoRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var assunto = await _assuntoService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = assunto.CodAs }, assunto);
    }

    /// <summary>
    /// Atualiza um assunto existente
    /// </summary>
    /// <param name="id">ID do assunto</param>
    /// <param name="request">Novos dados do assunto</param>
    /// <returns>Assunto atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AssuntoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AssuntoResponseDto>> Update(int id, [FromBody] AssuntoRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var assunto = await _assuntoService.UpdateAsync(id, request);
        return Ok(assunto);
    }

    /// <summary>
    /// Deleta um assunto
    /// </summary>
    /// <param name="id">ID do assunto</param>
    /// <returns>Confirmação de deleção</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _assuntoService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Busca assuntos por descrição
    /// </summary>
    /// <param name="description">Descrição ou parte da descrição</param>
    /// <returns>Lista de assuntos encontrados</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<AssuntoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AssuntoResponseDto>>> Search([FromQuery] string description)
    {
        var assuntos = await _assuntoService.SearchByDescriptionAsync(description);
        return Ok(assuntos);
    }
}



