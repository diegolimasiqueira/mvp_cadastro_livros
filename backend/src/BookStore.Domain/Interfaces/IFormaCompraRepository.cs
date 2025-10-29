using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

/// <summary>
/// Interface específica do repositório de FormaCompra
/// </summary>
public interface IFormaCompraRepository : IRepository<FormaCompra>
{
    Task<FormaCompra?> GetByDescriptionAsync(string description);
}



