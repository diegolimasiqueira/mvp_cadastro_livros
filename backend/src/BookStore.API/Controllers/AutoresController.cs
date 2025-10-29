using BookStore.Application.DTOs.Autor;
using BookStore.Application.DTOs.Common;
using BookStore.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

/// <summary>
/// Controller para gerenciamento de autores
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class AutoresController : ControllerBase
{
    private readonly IAutorService _autorService;
    private readonly IValidator<AutorRequestDto> _validator;

    public AutoresController(IAutorService autorService, IValidator<AutorRequestDto> validator)
    {
        _autorService = autorService;
        _validator = validator;
    }

    /// <summary>
    /// Obtém todos os autores
    /// </summary>
    /// <returns>Lista de autores</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AutorResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorResponseDto>>> GetAll()
    {
        var autores = await _autorService.GetAllAsync();
        return Ok(autores);
    }

    /// <summary>
    /// Obtém autores paginados
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    /// <returns>Resposta paginada com autores</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<AutorResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<AutorResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _autorService.GetPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um autor por ID
    /// </summary>
    /// <param name="id">ID do autor</param>
    /// <returns>Dados do autor</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AutorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponseDto>> GetById(int id)
    {
        var autor = await _autorService.GetByIdAsync(id);
        return Ok(autor);
    }

    /// <summary>
    /// Cria um novo autor
    /// </summary>
    /// <param name="request">Dados do autor</param>
    /// <returns>Autor criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AutorResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutorResponseDto>> Create([FromBody] AutorRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var autor = await _autorService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = autor.CodAu }, autor);
    }

    /// <summary>
    /// Atualiza um autor existente
    /// </summary>
    /// <param name="id">ID do autor</param>
    /// <param name="request">Novos dados do autor</param>
    /// <returns>Autor atualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AutorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponseDto>> Update(int id, [FromBody] AutorRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var autor = await _autorService.UpdateAsync(id, request);
        return Ok(autor);
    }

    /// <summary>
    /// Deleta um autor
    /// </summary>
    /// <param name="id">ID do autor</param>
    /// <returns>Confirmação de deleção</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _autorService.DeleteAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Busca autores por nome
    /// </summary>
    /// <param name="name">Nome ou parte do nome</param>
    /// <returns>Lista de autores encontrados</returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<AutorResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AutorResponseDto>>> Search([FromQuery] string name)
    {
        var autores = await _autorService.SearchByNameAsync(name);
        return Ok(autores);
    }
}



