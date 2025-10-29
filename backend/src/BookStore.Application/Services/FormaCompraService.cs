using BookStore.Application.DTOs.Common;
using BookStore.Application.DTOs.FormaCompra;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services;

/// <summary>
/// Serviço de negócio para FormaCompra
/// </summary>
public class FormaCompraService : IFormaCompraService
{
    private readonly IFormaCompraRepository _formaCompraRepository;

    public FormaCompraService(IFormaCompraRepository formaCompraRepository)
    {
        _formaCompraRepository = formaCompraRepository;
    }

    public async Task<IEnumerable<FormaCompraResponseDto>> GetAllAsync()
    {
        var formasCompra = await _formaCompraRepository.GetAllAsync();
        return formasCompra.Select(MapToResponseDto);
    }

    public async Task<PagedResponse<FormaCompraResponseDto>> GetPagedAsync(PagedRequest request)
    {
        var (formasCompra, totalCount) = await _formaCompraRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        var data = formasCompra.Select(MapToResponseDto);
        return new PagedResponse<FormaCompraResponseDto>(data, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<FormaCompraResponseDto> GetByIdAsync(int id)
    {
        var formaCompra = await _formaCompraRepository.GetByIdAsync(id);
        if (formaCompra == null)
        {
            throw new NotFoundException("Purchase Method", id);
        }

        return MapToResponseDto(formaCompra);
    }

    public async Task<FormaCompraResponseDto> CreateAsync(FormaCompraRequestDto request)
    {
        var formaCompra = new FormaCompra
        {
            Descricao = request.Descricao
        };

        // Validar regras de negócio
        formaCompra.Validate();

        var createdFormaCompra = await _formaCompraRepository.AddAsync(formaCompra);
        return MapToResponseDto(createdFormaCompra);
    }

    public async Task<FormaCompraResponseDto> UpdateAsync(int id, FormaCompraRequestDto request)
    {
        var formaCompra = await _formaCompraRepository.GetByIdAsync(id);
        if (formaCompra == null)
        {
            throw new NotFoundException("Purchase Method", id);
        }

        formaCompra.Descricao = request.Descricao;

        // Validar regras de negócio
        formaCompra.Validate();

        await _formaCompraRepository.UpdateAsync(formaCompra);
        return MapToResponseDto(formaCompra);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _formaCompraRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException("Purchase Method", id);
        }

        await _formaCompraRepository.DeleteAsync(id);
    }

    private static FormaCompraResponseDto MapToResponseDto(FormaCompra formaCompra)
    {
        return new FormaCompraResponseDto
        {
            CodFc = formaCompra.CodFc,
            Descricao = formaCompra.Descricao
        };
    }
}



