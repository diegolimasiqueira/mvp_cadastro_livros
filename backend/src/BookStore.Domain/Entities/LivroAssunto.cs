namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade de relacionamento N:N entre Livro e Assunto
/// </summary>
public class LivroAssunto
{
    public int Livro_CodI { get; set; }
    public int Assunto_CodAs { get; set; }

    // Navegação
    public virtual Livro Livro { get; set; } = null!;
    public virtual Assunto Assunto { get; set; } = null!;
}



