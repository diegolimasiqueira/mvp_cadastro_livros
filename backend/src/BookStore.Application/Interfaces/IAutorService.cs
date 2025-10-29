using BookStore.Application.DTOs.Autor;
using BookStore.Application.DTOs.Common;

namespace BookStore.Application.Interfaces;

/// <summary>
/// Interface para serviço de autores
/// </summary>
public interface IAutorService
{
    Task<IEnumerable<AutorResponseDto>> GetAllAsync();
    Task<PagedResponse<AutorResponseDto>> GetPagedAsync(PagedRequest request);
    Task<AutorResponseDto> GetByIdAsync(int id);
    Task<AutorResponseDto> CreateAsync(AutorRequestDto request);
    Task<AutorResponseDto> UpdateAsync(int id, AutorRequestDto request);
    Task DeleteAsync(int id);
    Task<IEnumerable<AutorResponseDto>> SearchByNameAsync(string name);
}



