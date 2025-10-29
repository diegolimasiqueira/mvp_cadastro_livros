namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade que representa o preço de um livro conforme a forma de compra
/// </summary>
public class LivroPreco
{
    public int CodLp { get; set; }
    public int Livro_CodI { get; set; }
    public int FormaCompra_CodFc { get; set; }
    public decimal Valor { get; set; }

    // Navegação
    public virtual Livro Livro { get; set; } = null!;
    public virtual FormaCompra FormaCompra { get; set; } = null!;

    // Validações de negócio
    public void Validate()
    {
        if (Valor < 0)
            throw new ArgumentException("Price cannot be negative");

        if (Valor > 999999.99m)
            throw new ArgumentException("Price cannot exceed 999999.99");
    }
}



