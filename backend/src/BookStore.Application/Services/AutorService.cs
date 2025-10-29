using BookStore.Application.DTOs.Autor;
using BookStore.Application.DTOs.Common;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services;

/// <summary>
/// Serviço de negócio para Autor
/// </summary>
public class AutorService : IAutorService
{
    private readonly IAutorRepository _autorRepository;

    public AutorService(IAutorRepository autorRepository)
    {
        _autorRepository = autorRepository;
    }

    public async Task<IEnumerable<AutorResponseDto>> GetAllAsync()
    {
        var autores = await _autorRepository.GetAllAsync();
        return autores.Select(MapToResponseDto);
    }

    public async Task<PagedResponse<AutorResponseDto>> GetPagedAsync(PagedRequest request)
    {
        var (autores, totalCount) = await _autorRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        var data = autores.Select(MapToResponseDto);
        return new PagedResponse<AutorResponseDto>(data, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<AutorResponseDto> GetByIdAsync(int id)
    {
        var autor = await _autorRepository.GetByIdAsync(id);
        if (autor == null)
        {
            throw new NotFoundException("Author", id);
        }

        return MapToResponseDto(autor);
    }

    public async Task<AutorResponseDto> CreateAsync(AutorRequestDto request)
    {
        var autor = new Autor
        {
            Nome = request.Nome
        };

        // Validar regras de negócio
        autor.Validate();

        var createdAutor = await _autorRepository.AddAsync(autor);
        return MapToResponseDto(createdAutor);
    }

    public async Task<AutorResponseDto> UpdateAsync(int id, AutorRequestDto request)
    {
        var autor = await _autorRepository.GetByIdAsync(id);
        if (autor == null)
        {
            throw new NotFoundException("Author", id);
        }

        autor.Nome = request.Nome;

        // Validar regras de negócio
        autor.Validate();

        await _autorRepository.UpdateAsync(autor);
        return MapToResponseDto(autor);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _autorRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException("Author", id);
        }

        await _autorRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<AutorResponseDto>> SearchByNameAsync(string name)
    {
        var autores = await _autorRepository.SearchByNameAsync(name);
        return autores.Select(MapToResponseDto);
    }

    private static AutorResponseDto MapToResponseDto(Autor autor)
    {
        return new AutorResponseDto
        {
            CodAu = autor.CodAu,
            Nome = autor.Nome
        };
    }
}



