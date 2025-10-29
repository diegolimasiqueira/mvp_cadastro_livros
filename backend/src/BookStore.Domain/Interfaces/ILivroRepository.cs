using BookStore.Domain.Entities;

namespace BookStore.Domain.Interfaces;

/// <summary>
/// Interface específica do repositório de Livro com operações especializadas
/// </summary>
public interface ILivroRepository : IRepository<Livro>
{
    Task<IEnumerable<Livro>> GetWithAuthorsAndSubjectsAsync();
    Task<Livro?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Livro>> SearchByTitleAsync(string title);
    Task<IEnumerable<Livro>> GetByAuthorAsync(int authorId);
    Task<IEnumerable<Livro>> GetBySubjectAsync(int subjectId);
}



