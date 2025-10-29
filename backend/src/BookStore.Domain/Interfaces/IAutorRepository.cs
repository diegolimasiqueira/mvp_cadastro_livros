using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

/// <summary>
/// Interface específica do repositório de Autor
/// </summary>
public interface IAutorRepository : IRepository<Autor>
{
    Task<IEnumerable<Autor>> SearchByNameAsync(string name);
    Task<Autor?> GetByIdWithBooksAsync(int id);
}



