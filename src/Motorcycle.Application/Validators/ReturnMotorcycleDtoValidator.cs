using FluentValidation;
using Motorcycle.Application.DTOs.Rental;

namespace Motorcycle.Application.Validators;

public class ReturnMotorcycleDtoValidator : AbstractValidator<ReturnMotorcycleDto>
{
    public ReturnMotorcycleDtoValidator()
    {
        RuleFor(x => x.ReturnDate)
            .NotEmpty().WithMessage("A data de devolução é obrigatória.")
            .Must(BeNotInFuture).WithMessage("A data de devolução não pode ser no futuro.");
    }

    private bool BeNotInFuture(DateOnly date)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return date <= today;
    }
}