namespace BookStore.Application.DTOs.Livro;

/// <summary>
/// DTO para criação/atualização de livro
/// </summary>
public class LivroRequestDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;
    
    // IDs dos autores relacionados
    public List<int> AutoresIds { get; set; } = new();
    
    // IDs dos assuntos relacionados
    public List<int> AssuntosIds { get; set; } = new();
    
    // Preços por forma de compra
    public List<LivroPrecoRequestDto> Precos { get; set; } = new();
}

/// <summary>
/// DTO para preço de livro na requisição
/// </summary>
public class LivroPrecoRequestDto
{
    public int FormaCompraId { get; set; }
    public decimal Valor { get; set; }
}



