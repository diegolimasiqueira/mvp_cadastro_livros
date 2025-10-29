using BookStore.Application.DTOs.FormaCompra;
using FluentValidation;

namespace BookStore.Application.Validators;

/// <summary>
/// Validador para requisição de forma de compra
/// </summary>
public class FormaCompraRequestValidator : AbstractValidator<FormaCompraRequestDto>
{
    public FormaCompraRequestValidator()
    {
        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("Purchase method description is required")
            .MaximumLength(30).WithMessage("Purchase method description cannot exceed 30 characters");
    }
}



