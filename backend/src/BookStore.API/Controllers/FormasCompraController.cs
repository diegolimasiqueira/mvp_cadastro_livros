using BookStore.Application.DTOs.Common;
using BookStore.Application.DTOs.FormaCompra;
using BookStore.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.API.Controllers;

/// <summary>
/// Controller para gerenciamento de formas de compra
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class FormasCompraController : ControllerBase
{
    private readonly IFormaCompraService _formaCompraService;
    private readonly IValidator<FormaCompraRequestDto> _validator;

    public FormasCompraController(IFormaCompraService formaCompraService, IValidator<FormaCompraRequestDto> validator)
    {
        _formaCompraService = formaCompraService;
        _validator = validator;
    }

    /// <summary>
    /// Obtém todas as formas de compra
    /// </summary>
    /// <returns>Lista de formas de compra</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FormaCompraResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FormaCompraResponseDto>>> GetAll()
    {
        var formasCompra = await _formaCompraService.GetAllAsync();
        return Ok(formasCompra);
    }

    /// <summary>
    /// Obtém formas de compra paginadas
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10, máximo: 100)</param>
    /// <returns>Resposta paginada com formas de compra</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<FormaCompraResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<FormaCompraResponseDto>>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var request = new PagedRequest { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _formaCompraService.GetPagedAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Obtém uma forma de compra por ID
    /// </summary>
    /// <param name="id">ID da forma de compra</param>
    /// <returns>Dados da forma de compra</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(FormaCompraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormaCompraResponseDto>> GetById(int id)
    {
        var formaCompra = await _formaCompraService.GetByIdAsync(id);
        return Ok(formaCompra);
    }

    /// <summary>
    /// Cria uma nova forma de compra
    /// </summary>
    /// <param name="request">Dados da forma de compra</param>
    /// <returns>Forma de compra criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FormaCompraResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FormaCompraResponseDto>> Create([FromBody] FormaCompraRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var formaCompra = await _formaCompraService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = formaCompra.CodFc }, formaCompra);
    }

    /// <summary>
    /// Atualiza uma forma de compra existente
    /// </summary>
    /// <param name="id">ID da forma de compra</param>
    /// <param name="request">Novos dados da forma de compra</param>
    /// <returns>Forma de compra atualizada</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(FormaCompraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FormaCompraResponseDto>> Update(int id, [FromBody] FormaCompraRequestDto request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var formaCompra = await _formaCompraService.UpdateAsync(id, request);
        return Ok(formaCompra);
    }

    /// <summary>
    /// Deleta uma forma de compra
    /// </summary>
    /// <param name="id">ID da forma de compra</param>
    /// <returns>Confirmação de deleção</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await _formaCompraService.DeleteAsync(id);
        return NoContent();
    }
}



