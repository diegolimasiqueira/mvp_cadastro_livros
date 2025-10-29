using BookStore.Application.DTOs.Assunto;
using FluentValidation;

namespace BookStore.Application.Validators;

/// <summary>
/// Validador para requisição de assunto
/// </summary>
public class AssuntoRequestValidator : AbstractValidator<AssuntoRequestDto>
{
    public AssuntoRequestValidator()
    {
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Subject description is required")
            .MaximumLength(20).WithMessage("Subject description cannot exceed 20 characters");
    }
}



