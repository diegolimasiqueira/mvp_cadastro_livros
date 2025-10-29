using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Autor
/// </summary>
public class AutorRepository : Repository<Autor>, IAutorRepository
{
    public AutorRepository(BookStoreDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Autor>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(a => EF.Functions.ILike(a.Nome, $"%{name}%"))
            .ToListAsync();
    }

    public async Task<Autor?> GetByIdWithBooksAsync(int id)
    {
        return await _dbSet
            .Include(a => a.LivroAutores)
                .ThenInclude(la => la.Livro)
            .FirstOrDefaultAsync(a => a.CodAu == id);
    }
}



