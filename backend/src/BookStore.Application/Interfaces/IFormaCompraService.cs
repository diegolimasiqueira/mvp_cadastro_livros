using BookStore.Application.DTOs.Common;
using BookStore.Application.DTOs.FormaCompra;

namespace BookStore.Application.Interfaces;

/// <summary>
/// Interface para serviço de formas de compra
/// </summary>
public interface IFormaCompraService
{
    Task<IEnumerable<FormaCompraResponseDto>> GetAllAsync();
    Task<PagedResponse<FormaCompraResponseDto>> GetPagedAsync(PagedRequest request);
    Task<FormaCompraResponseDto> GetByIdAsync(int id);
    Task<FormaCompraResponseDto> CreateAsync(FormaCompraRequestDto request);
    Task<FormaCompraResponseDto> UpdateAsync(int id, FormaCompraRequestDto request);
    Task DeleteAsync(int id);
}



