namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade que representa uma forma de compra (balcão, internet, self-service, etc)
/// </summary>
public class FormaCompra
{
    public int CodFc { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Navegação para relacionamentos
    public virtual ICollection<LivroPreco> LivroPrecos { get; set; } = new List<LivroPreco>();

    // Validações de negócio
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Descricao))
            throw new ArgumentException("Purchase method description is required");

        if (Descricao.Length > 30)
            throw new ArgumentException("Purchase method description cannot exceed 30 characters");
    }
}



