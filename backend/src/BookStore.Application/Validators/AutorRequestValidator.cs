using BookStore.Application.DTOs.Autor;
using FluentValidation;

namespace BookStore.Application.Validators;

/// <summary>
/// Validador para requisição de autor
/// </summary>
public class AutorRequestValidator : AbstractValidator<AutorRequestDto>
{
    public AutorRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Author name is required")
            .MaximumLength(40).WithMessage("Author name cannot exceed 40 characters");
    }
}



