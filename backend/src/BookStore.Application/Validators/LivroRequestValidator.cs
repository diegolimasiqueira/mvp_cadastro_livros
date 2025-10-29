using BookStore.Application.DTOs.Livro;
using FluentValidation;

namespace BookStore.Application.Validators;

/// <summary>
/// Validador para requisição de livro
/// </summary>
public class LivroRequestValidator : AbstractValidator<LivroRequestDto>
{
    public LivroRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(40).WithMessage("Title cannot exceed 40 characters");

        RuleFor(x => x.Editora)
            .NotEmpty().WithMessage("Publisher is required")
            .MaximumLength(40).WithMessage("Publisher cannot exceed 40 characters");

        RuleFor(x => x.Edicao)
            .GreaterThan(0).WithMessage("Edition must be greater than zero");

        RuleFor(x => x.AnoPublicacao)
            .NotEmpty().WithMessage("Publication year is required")
            .Length(4).WithMessage("Publication year must be 4 characters")
            .Matches(@"^\d{4}$").WithMessage("Publication year must be a valid year");

        RuleFor(x => x.AutoresIds)
            .NotEmpty().WithMessage("At least one author is required");

        RuleFor(x => x.AssuntosIds)
            .NotEmpty().WithMessage("At least one subject is required");

        RuleForEach(x => x.Precos)
            .SetValidator(new LivroPrecoRequestValidator());
    }
}

/// <summary>
/// Validador para preço de livro
/// </summary>
public class LivroPrecoRequestValidator : AbstractValidator<LivroPrecoRequestDto>
{
    public LivroPrecoRequestValidator()
    {
        RuleFor(x => x.FormaCompraId)
            .GreaterThan(0).WithMessage("Purchase method is required");

        RuleFor(x => x.Valor)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative")
            .LessThan(1000000).WithMessage("Price cannot exceed 999,999.99");
    }
}



