using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Assunto
/// </summary>
public class AssuntoRepository : Repository<Assunto>, IAssuntoRepository
{
    public AssuntoRepository(BookStoreDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Assunto>> SearchByDescriptionAsync(string description)
    {
        return await _dbSet
            .Where(a => EF.Functions.ILike(a.Descricao, $"%{description}%"))
            .ToListAsync();
    }

    public async Task<Assunto?> GetByIdWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(a => a.LivroAssuntos)
                .ThenInclude(la => la.Livro)
            .FirstOrDefaultAsync(a => a.CodAs == id);
    }
}



