namespace BookStore.Application.DTOs.Livro;

/// <summary>
/// DTO para preço de livro por forma de compra
/// </summary>
public class LivroPrecoDto
{
    public int CodFc { get; set; }
    public string FormaCompraDescricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}



