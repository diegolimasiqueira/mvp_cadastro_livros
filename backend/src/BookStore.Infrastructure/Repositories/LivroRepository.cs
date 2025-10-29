using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Livro com operações especializadas
/// </summary>
public class LivroRepository : Repository<Livro>, ILivroRepository
{
    public LivroRepository(BookStoreDbContext context) : base(context)
    {
    }

    public override async Task<Livro?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(la => la.Assunto)
            .Include(l => l.LivroPrecos)
                .ThenInclude(lp => lp.FormaCompra)
            .FirstOrDefaultAsync(l => l.CodI == id);
    }

    public override async Task<(IEnumerable<Livro> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(la => la.Assunto)
            .Include(l => l.LivroPrecos)
                .ThenInclude(lp => lp.FormaCompra)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Livro>> GetWithAuthorsAndSubjectsAsync()
    {
        return await _dbSet
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(la => la.Assunto)
            .Include(l => l.LivroPrecos)
                .ThenInclude(lp => lp.FormaCompra)
            .ToListAsync();
    }

    public async Task<Livro?> GetByIdWithDetailsAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IEnumerable<Livro>> SearchByTitleAsync(string title)
    {
        return await _dbSet
            .Where(l => EF.Functions.ILike(l.Titulo, $"%{title}%"))
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(la => la.Assunto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Livro>> GetByAuthorAsync(int authorId)
    {
        return await _dbSet
            .Where(l => l.LivroAutores.Any(la => la.Autor_CodAu == authorId))
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(la => la.Assunto)
            .ToListAsync();
    }

    public async Task<IEnumerable<Livro>> GetBySubjectAsync(int subjectId)
    {
        return await _dbSet
            .Where(l => l.LivroAssuntos.Any(la => la.Assunto_CodAs == subjectId))
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(la => la.Assunto)
            .ToListAsync();
    }
}



