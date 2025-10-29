using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

/// <summary>
/// Interface específica do repositório de Assunto
/// </summary>
public interface IAssuntoRepository : IRepository<Assunto>
{
    Task<IEnumerable<Assunto>> SearchByDescriptionAsync(string description);
    Task<Assunto?> GetByIdWithBooksAsync(int id);
}



