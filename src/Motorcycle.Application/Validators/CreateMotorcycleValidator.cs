using FluentValidation;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Domain.Interfaces.Repositories;

namespace Motorcycle.Application.Validators;

public class CreateMotorcycleValidator : AbstractValidator<CreateMotorcycleDto>
{
    private readonly IMotorcycleRepository _motorcycleRepository;

    public CreateMotorcycleValidator(IMotorcycleRepository motorcycleRepository)
    {
        _motorcycleRepository = motorcycleRepository;

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("O modelo da motocicleta é obrigatório.")
            .MaximumLength(100).WithMessage("O modelo não pode exceder 100 caracteres.");

        RuleFor(x => x.Year)
            .NotEmpty().WithMessage("O ano da motocicleta é obrigatório.")
            .InclusiveBetween(1900, DateTime.Now.Year + 1)
            .WithMessage($"O ano deve estar entre 1900 e {DateTime.Now.Year + 1}.");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("A placa da motocicleta é obrigatória.")
            .MaximumLength(8).WithMessage("A placa não pode exceder 8 caracteres.")
            .Matches(@"^[A-Za-z]{3}\d[A-Za-z0-9]\d{2}$|^[A-Za-z]{3}\d{4}$|^[A-Za-z]{3}-\d{4}$")
            .WithMessage("A placa deve estar em um formato válido (AAA9999, AAA9A99 ou AAA-9999).")
            .MustAsync(BeUniqueLicensePlate).WithMessage("Esta placa já está cadastrada.");
    }

    private async Task<bool> BeUniqueLicensePlate(string licensePlate, CancellationToken cancellationToken)
    {
        return !await _motorcycleRepository.ExistsByLicensePlateAsync(licensePlate, cancellationToken);
    }
}