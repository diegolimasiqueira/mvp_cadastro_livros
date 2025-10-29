using BookStore.Domain.Entities;
using BookStore.Domain.Interfaces;
using BookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de FormaCompra
/// </summary>
public class FormaCompraRepository : Repository<FormaCompra>, IFormaCompraRepository
{
    public FormaCompraRepository(BookStoreDbContext context) : base(context)
    {
    }

    public async Task<FormaCompra?> GetByDescriptionAsync(string description)
    {
        return await _dbSet
            .FirstOrDefaultAsync(fc => fc.Descricao == description);
    }
}



