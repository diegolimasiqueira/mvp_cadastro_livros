using BookStore.Application.DTOs.Assunto;
using BookStore.Application.DTOs.Autor;
using BookStore.Application.DTOs.Common;
using BookStore.Application.DTOs.Livro;
using BookStore.Application.Exceptions;
using BookStore.Application.Interfaces;
using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;

namespace BookStore.Application.Services;

/// <summary>
/// Serviço de negócio para Livro
/// </summary>
public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IAssuntoRepository _assuntoRepository;
    private readonly IFormaCompraRepository _formaCompraRepository;

    public LivroService(
        ILivroRepository livroRepository,
        IAutorRepository autorRepository,
        IAssuntoRepository assuntoRepository,
        IFormaCompraRepository formaCompraRepository)
    {
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
        _assuntoRepository = assuntoRepository;
        _formaCompraRepository = formaCompraRepository;
    }

    public async Task<IEnumerable<LivroResponseDto>> GetAllAsync()
    {
        var livros = await _livroRepository.GetWithAuthorsAndSubjectsAsync();
        return livros.Select(MapToResponseDto);
    }

    public async Task<PagedResponse<LivroResponseDto>> GetPagedAsync(PagedRequest request)
    {
        var (livros, totalCount) = await _livroRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        var data = livros.Select(MapToResponseDto);
        return new PagedResponse<LivroResponseDto>(data, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<LivroResponseDto> GetByIdAsync(int id)
    {
        var livro = await _livroRepository.GetByIdWithDetailsAsync(id);
        if (livro == null)
        {
            throw new NotFoundException("Book", id);
        }

        return MapToResponseDto(livro);
    }

    public async Task<LivroResponseDto> CreateAsync(LivroRequestDto request)
    {
        // Validar se autores existem
        await ValidateAutoresExist(request.AutoresIds);

        // Validar se assuntos existem
        await ValidateAssuntosExist(request.AssuntosIds);

        // Validar se formas de compra existem
        await ValidateFormasCompraExist(request.Precos.Select(p => p.FormaCompraId).ToList());

        var livro = new Livro
        {
            Titulo = request.Titulo,
            Editora = request.Editora,
            Edicao = request.Edicao,
            AnoPublicacao = request.AnoPublicacao
        };

        // Validar regras de negócio
        livro.Validate();

        // Adicionar relacionamentos com autores
        foreach (var autorId in request.AutoresIds)
        {
            livro.LivroAutores.Add(new LivroAutor
            {
                Autor_CodAu = autorId
            });
        }

        // Adicionar relacionamentos com assuntos
        foreach (var assuntoId in request.AssuntosIds)
        {
            livro.LivroAssuntos.Add(new LivroAssunto
            {
                Assunto_CodAs = assuntoId
            });
        }

        // Adicionar preços
        foreach (var preco in request.Precos)
        {
            var livroPreco = new LivroPreco
            {
                FormaCompra_CodFc = preco.FormaCompraId,
                Valor = preco.Valor
            };
            livroPreco.Validate();
            livro.LivroPrecos.Add(livroPreco);
        }

        var createdLivro = await _livroRepository.AddAsync(livro);
        
        // Recarregar com relacionamentos completos
        var livroCompleto = await _livroRepository.GetByIdWithDetailsAsync(createdLivro.CodI);
        return MapToResponseDto(livroCompleto!);
    }

    public async Task<LivroResponseDto> UpdateAsync(int id, LivroRequestDto request)
    {
        var livro = await _livroRepository.GetByIdWithDetailsAsync(id);
        if (livro == null)
        {
            throw new NotFoundException("Book", id);
        }

        // Validar se autores existem
        await ValidateAutoresExist(request.AutoresIds);

        // Validar se assuntos existem
        await ValidateAssuntosExist(request.AssuntosIds);

        // Validar se formas de compra existem
        await ValidateFormasCompraExist(request.Precos.Select(p => p.FormaCompraId).ToList());

        // Atualizar dados básicos
        livro.Titulo = request.Titulo;
        livro.Editora = request.Editora;
        livro.Edicao = request.Edicao;
        livro.AnoPublicacao = request.AnoPublicacao;

        // Validar regras de negócio
        livro.Validate();

        // Atualizar relacionamentos com autores
        livro.LivroAutores.Clear();
        foreach (var autorId in request.AutoresIds)
        {
            livro.LivroAutores.Add(new LivroAutor
            {
                Livro_CodI = livro.CodI,
                Autor_CodAu = autorId
            });
        }

        // Atualizar relacionamentos com assuntos
        livro.LivroAssuntos.Clear();
        foreach (var assuntoId in request.AssuntosIds)
        {
            livro.LivroAssuntos.Add(new LivroAssunto
            {
                Livro_CodI = livro.CodI,
                Assunto_CodAs = assuntoId
            });
        }

        // Atualizar preços
        livro.LivroPrecos.Clear();
        foreach (var preco in request.Precos)
        {
            var livroPreco = new LivroPreco
            {
                Livro_CodI = livro.CodI,
                FormaCompra_CodFc = preco.FormaCompraId,
                Valor = preco.Valor
            };
            livroPreco.Validate();
            livro.LivroPrecos.Add(livroPreco);
        }

        await _livroRepository.UpdateAsync(livro);
        
        // Recarregar com relacionamentos completos
        var livroAtualizado = await _livroRepository.GetByIdWithDetailsAsync(id);
        return MapToResponseDto(livroAtualizado!);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _livroRepository.ExistsAsync(id);
        if (!exists)
        {
            throw new NotFoundException("Book", id);
        }

        await _livroRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<LivroResponseDto>> SearchByTitleAsync(string title)
    {
        var livros = await _livroRepository.SearchByTitleAsync(title);
        return livros.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<LivroResponseDto>> GetByAuthorAsync(int authorId)
    {
        var livros = await _livroRepository.GetByAuthorAsync(authorId);
        return livros.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<LivroResponseDto>> GetBySubjectAsync(int subjectId)
    {
        var livros = await _livroRepository.GetBySubjectAsync(subjectId);
        return livros.Select(MapToResponseDto);
    }

    private async Task ValidateAutoresExist(List<int> autoresIds)
    {
        foreach (var autorId in autoresIds)
        {
            var exists = await _autorRepository.ExistsAsync(autorId);
            if (!exists)
            {
                throw new BusinessException($"Author with id {autorId} does not exist");
            }
        }
    }

    private async Task ValidateAssuntosExist(List<int> assuntosIds)
    {
        foreach (var assuntoId in assuntosIds)
        {
            var exists = await _assuntoRepository.ExistsAsync(assuntoId);
            if (!exists)
            {
                throw new BusinessException($"Subject with id {assuntoId} does not exist");
            }
        }
    }

    private async Task ValidateFormasCompraExist(List<int> formasCompraIds)
    {
        foreach (var formaCompraId in formasCompraIds)
        {
            var exists = await _formaCompraRepository.ExistsAsync(formaCompraId);
            if (!exists)
            {
                throw new BusinessException($"Purchase method with id {formaCompraId} does not exist");
            }
        }
    }

    private static LivroResponseDto MapToResponseDto(Livro livro)
    {
        return new LivroResponseDto
        {
            CodI = livro.CodI,
            Titulo = livro.Titulo,
            Editora = livro.Editora,
            Edicao = livro.Edicao,
            AnoPublicacao = livro.AnoPublicacao,
            Autores = livro.LivroAutores.Select(la => new AutorResponseDto
            {
                CodAu = la.Autor.CodAu,
                Nome = la.Autor.Nome
            }).ToList(),
            Assuntos = livro.LivroAssuntos.Select(la => new AssuntoResponseDto
            {
                CodAs = la.Assunto.CodAs,
                Descricao = la.Assunto.Descricao
            }).ToList(),
            Precos = livro.LivroPrecos.Select(lp => new LivroPrecoDto
            {
                CodFc = lp.FormaCompra.CodFc,
                FormaCompraDescricao = lp.FormaCompra.Descricao,
                Valor = lp.Valor
            }).ToList()
        };
    }
}



