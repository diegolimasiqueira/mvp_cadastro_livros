namespace BookStore.Domain.Entities.Views;

/// <summary>
/// Entidade que mapeia a view vw_livros_por_autor
/// </summary>
public class VwLivrosPorAutor
{
    public int LivroCodigo { get; set; }
    public string LivroTitulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;
    public int AutorCodigo { get; set; }
    public string AutorNome { get; set; } = string.Empty;
    public string? Assuntos { get; set; }
    public string? FormasCompra { get; set; }
}

