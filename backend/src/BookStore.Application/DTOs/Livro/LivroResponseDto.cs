using BookStore.Application.DTOs.Assunto;
using BookStore.Application.DTOs.Autor;

namespace BookStore.Application.DTOs.Livro;

/// <summary>
/// DTO para resposta de livro
/// </summary>
public class LivroResponseDto
{
    public int CodI { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Editora { get; set; } = string.Empty;
    public int Edicao { get; set; }
    public string AnoPublicacao { get; set; } = string.Empty;
    
    // Autores relacionados
    public List<AutorResponseDto> Autores { get; set; } = new();
    
    // Assuntos relacionados
    public List<AssuntoResponseDto> Assuntos { get; set; } = new();
    
    // Preços por forma de compra
    public List<LivroPrecoDto> Precos { get; set; } = new();
}



