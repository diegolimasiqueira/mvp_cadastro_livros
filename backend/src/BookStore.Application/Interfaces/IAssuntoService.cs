using BookStore.Application.DTOs.Assunto;
using BookStore.Application.DTOs.Common;

namespace BookStore.Application.Interfaces;

/// <summary>
/// Interface para serviço de assuntos
/// </summary>
public interface IAssuntoService
{
    Task<IEnumerable<AssuntoResponseDto>> GetAllAsync();
    Task<PagedResponse<AssuntoResponseDto>> GetPagedAsync(PagedRequest request);
    Task<AssuntoResponseDto> GetByIdAsync(int id);
    Task<AssuntoResponseDto> CreateAsync(AssuntoRequestDto request);
    Task<AssuntoResponseDto> UpdateAsync(int id, AssuntoRequestDto request);
    Task DeleteAsync(int id);
    Task<IEnumerable<AssuntoResponseDto>> SearchByDescriptionAsync(string description);
}



