namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade que representa um autor no sistema
/// </summary>
public class Autor
{
    public int CodAu { get; set; }
    public string Nome { get; set; } = string.Empty;

    // Navegação para relacionamentos N:N
    public virtual ICollection<LivroAutor> LivroAutores { get; set; } = new List<LivroAutor>();

    // Validações de negócio
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new ArgumentException("Author name is required");

        if (Nome.Length > 40)
            throw new ArgumentException("Author name cannot exceed 40 characters");
    }
}



