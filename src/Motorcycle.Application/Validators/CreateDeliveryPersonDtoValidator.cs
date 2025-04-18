using FluentValidation;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Domain.Interfaces.Repositories;
using System;

namespace Motorcycle.Application.Validators;

public class CreateDeliveryPersonDtoValidator : AbstractValidator<CreateDeliveryPersonDto>
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;

    public CreateDeliveryPersonDtoValidator(IDeliveryPersonRepository deliveryPersonRepository)
    {
        _deliveryPersonRepository = deliveryPersonRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do entregador é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .MustAsync(BeUniqueCnpj).WithMessage("O CNPJ informado já está cadastrado.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .Must(BeAtLeast18YearsOld).WithMessage("O entregador deve ter no mínimo 18 anos.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("O número da CNH é obrigatório.")
            .Length(11).WithMessage("O número da CNH deve ter 11 dígitos.")
            .Matches(@"^\d{11}$").WithMessage("O número da CNH deve conter apenas dígitos.")
            .MustAsync(BeUniqueLicenseNumber).WithMessage("O número da CNH informado já está cadastrado.");

        RuleFor(x => x.LicenseType)
            .IsInEnum().WithMessage("O tipo de CNH informado é inválido.");
    }

    private async Task<bool> BeUniqueCnpj(string cnpj, CancellationToken cancellationToken)
    {
        return !await _deliveryPersonRepository.ExistsByCnpjAsync(cnpj, cancellationToken);
    }

    private async Task<bool> BeUniqueLicenseNumber(string licenseNumber, CancellationToken cancellationToken)
    {
        return !await _deliveryPersonRepository.ExistsByDriverLicenseNumberAsync(licenseNumber, cancellationToken);
    }

    private bool BeAtLeast18YearsOld(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.AddYears(age) > today)
            age--;

        return age >= 18;
    }
}