namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade que representa um livro no sistema
/// </summary>
public class Livro
{
    public int CodI { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;

    // Navegação para relacionamentos N:N
    public virtual ICollection<LivroAutor> LivroAutores { get; set; } = new List<LivroAutor>();
    public virtual ICollection<LivroAssunto> LivroAssuntos { get; set; } = new List<LivroAssunto>();
    public virtual ICollection<LivroPreco> LivroPrecos { get; set; } = new List<LivroPreco>();

    // Validações de negócio
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
            throw new ArgumentException("Title is required");

        if (Titulo.Length > 40)
            throw new ArgumentException("Title cannot exceed 40 characters");

        if (string.IsNullOrWhiteSpace(Editora))
            throw new ArgumentException("Publisher is required");

        if (Editora.Length > 40)
            throw new ArgumentException("Publisher cannot exceed 40 characters");

        if (Edicao <= 0)
            throw new ArgumentException("Edition must be greater than zero");

        if (string.IsNullOrWhiteSpace(AnoPublicacao))
            throw new ArgumentException("Publication year is required");

        if (AnoPublicacao.Length != 4)
            throw new ArgumentException("Publication year must be 4 characters");

        if (!int.TryParse(AnoPublicacao, out int year) || year < 1000 || year > 9999)
            throw new ArgumentException("Publication year must be a valid year");
    }
}



