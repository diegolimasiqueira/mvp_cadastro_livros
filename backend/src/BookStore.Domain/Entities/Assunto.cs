namespace BookStore.Domain.Entities;

/// <summary>
/// Entidade que representa um assunto (categoria) no sistema
/// </summary>
public class Assunto
{
    public int CodAs { get; set; }
    public string Descricao { get; set; } = string.Empty;

    // Navegação para relacionamentos N:N
    public virtual ICollection<LivroAssunto> LivroAssuntos { get; set; } = new List<LivroAssunto>();

    // Validações de negócio
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Descricao))
            throw new ArgumentException("Subject description is required");

        if (Descricao.Length > 20)
            throw new ArgumentException("Subject description cannot exceed 20 characters");
    }
}



