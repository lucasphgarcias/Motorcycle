using FluentValidation;
using Motorcycle.Application.DTOs.Motorcycle;
using System.Text.RegularExpressions;

namespace Motorcycle.Application.Validators;

public class UpdateMotorcycleDtoValidator : AbstractValidator<UpdateMotorcycleDto>
{
    public UpdateMotorcycleDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id da motocicleta é obrigatório");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Modelo da motocicleta é obrigatório");

        RuleFor(x => x.Brand)
            .NotEmpty()
            .WithMessage("Marca da motocicleta é obrigatória");

        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("Cor da motocicleta é obrigatória");

        RuleFor(x => x.ManufacturingYear)
            .GreaterThan(0)
            .WithMessage("Ano de fabricação deve ser maior que zero")
            .LessThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Ano de fabricação não pode ser futuro");

        RuleFor(x => x.PlateNumber)
            .NotEmpty()
            .WithMessage("Número da placa é obrigatório")
            .Matches(new Regex(@"^[A-Z]{3}-?\d{4}$"))
            .WithMessage("Formato de placa inválido. Formato esperado: ABC1234 ou ABC-1234");

        RuleFor(x => x.DailyRate)
            .GreaterThan(0)
            .WithMessage("Valor diário deve ser maior que zero");
    }
} 