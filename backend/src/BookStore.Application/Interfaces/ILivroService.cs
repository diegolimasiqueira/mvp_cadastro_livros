using BookStore.Application.DTOs.Common;
using BookStore.Application.DTOs.Livro;

namespace BookStore.Application.Interfaces;

/// <summary>
/// Interface para serviço de livros
/// </summary>
public interface ILivroService
{
    Task<IEnumerable<LivroResponseDto>> GetAllAsync();
    Task<PagedResponse<LivroResponseDto>> GetPagedAsync(PagedRequest request);
    Task<LivroResponseDto> GetByIdAsync(int id);
    Task<LivroResponseDto> CreateAsync(LivroRequestDto request);
    Task<LivroResponseDto> UpdateAsync(int id, LivroRequestDto request);
    Task DeleteAsync(int id);
    Task<IEnumerable<LivroResponseDto>> SearchByTitleAsync(string title);
    Task<IEnumerable<LivroResponseDto>> GetByAuthorAsync(int authorId);
    Task<IEnumerable<LivroResponseDto>> GetBySubjectAsync(int subjectId);
}



