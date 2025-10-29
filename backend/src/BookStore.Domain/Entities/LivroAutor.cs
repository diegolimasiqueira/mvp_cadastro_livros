namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade de relacionamento N:N entre Livro e Autor
/// </summary>
public class LivroAutor
{
    public int Livro_CodI { get; set; }
    public int Autor_CodAu { get; set; }

    // Navegação
    public virtual Livro Livro { get; set; } = null!;
    public virtual Autor Autor { get; set; } = null!;
}



