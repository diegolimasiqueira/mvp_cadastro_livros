using BookStore.Application.DTOs.Assunto;
using BookStore.Application.DTOs.Common;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services;

/// <summary>
/// Serviço de negócio para Assunto
/// </summary>
public class AssuntoService : IAssuntoService
{
    private readonly IAssuntoRepository _assuntoRepository;

    public AssuntoService(IAssuntoRepository assuntoRepository)
    {
        _assuntoRepository = assuntoRepository;
    }

    public async Task<IEnumerable<AssuntoResponseDto>> GetAllAsync()
    {
        var assuntos = await _assuntoRepository.GetAllAsync();
        return assuntos.Select(MapToResponseDto);
    }

    public async Task<PagedResponse<AssuntoResponseDto>> GetPagedAsync(PagedRequest request)
    {
        var (assuntos, totalCount) = await _assuntoRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        var data = assuntos.Select(MapToResponseDto);
        return new PagedResponse<AssuntoResponseDto>(data, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<AssuntoResponseDto> GetByIdAsync(int id)
    {
        var assunto = await _assuntoRepository.GetByIdAsync(id);
        if (assunto == null)
        {
            throw new NotFoundException("Subject", id);
        }

        return MapToResponseDto(assunto);
    }

    public async Task<AssuntoResponseDto> CreateAsync(AssuntoRequestDto request)
    {
        var assunto = new Assunto
        {
            Descricao = request.Descricao
        };

        // Validar regras de negócio
        assunto.Validate();

        var createdAssunto = await _assuntoRepository.AddAsync(assunto);
        return MapToResponseDto(createdAssunto);
    }

    public async Task<AssuntoResponseDto> UpdateAsync(int id, AssuntoRequestDto request)
    {
        var assunto = await _assuntoRepository.GetByIdAsync(id);
        if (assunto == null)
        {
            throw new NotFoundException("Subject", id);
        }

        assunto.Descricao = request.Descricao;

        // Validar regras de negócio
        assunto.Validate();

        await _assuntoRepository.UpdateAsync(assunto);
        return MapToResponseDto(assunto);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _assuntoRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException("Subject", id);
        }

        await _assuntoRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<AssuntoResponseDto>> SearchByDescriptionAsync(string description)
    {
        var assuntos = await _assuntoRepository.SearchByDescriptionAsync(description);
        return assuntos.Select(MapToResponseDto);
    }

    private static AssuntoResponseDto MapToResponseDto(Assunto assunto)
    {
        return new AssuntoResponseDto
        {
            CodAs = assunto.CodAs,
            Descricao = assunto.Descricao
        };
    }
}



