using FluentValidation;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Interfaces.Repositories;

namespace Motorcycle.Application.Validators;

public class CreateRentalDtoValidator : AbstractValidator<CreateRentalDto>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IRentalRepository _rentalRepository;

    public CreateRentalDtoValidator(
        IMotorcycleRepository motorcycleRepository,
        IDeliveryPersonRepository deliveryPersonRepository,
        IRentalRepository rentalRepository)
    {
        _motorcycleRepository = motorcycleRepository;
        _deliveryPersonRepository = deliveryPersonRepository;
        _rentalRepository = rentalRepository;

        RuleFor(x => x.MotorcycleId)
            .NotEmpty().WithMessage("O ID da moto é obrigatório.")
            .MustAsync(MotorcycleExists).WithMessage("A moto informada não existe.")
            .MustAsync(MotorcycleNotInUse).WithMessage("A moto informada já está em uso.");

        RuleFor(x => x.DeliveryPersonId)
            .NotEmpty().WithMessage("O ID do entregador é obrigatório.")
            .MustAsync(DeliveryPersonExists).WithMessage("O entregador informado não existe.")
            .MustAsync(DeliveryPersonCanRentMotorcycle).WithMessage("O entregador não está habilitado para dirigir motos.")
            .MustAsync(DeliveryPersonNotAlreadyRenting).WithMessage("O entregador já possui uma locação ativa.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("A data de início é obrigatória.")
            .Must(BeAtLeastTomorrow).WithMessage("A data de início deve ser, no mínimo, o dia seguinte à data atual.");

        RuleFor(x => x.PlanType)
            .IsInEnum().WithMessage("O plano de aluguel informado é inválido.");
    }

    private async Task<bool> MotorcycleExists(Guid id, CancellationToken cancellationToken)
    {
        return await _motorcycleRepository.GetByIdAsync(id, cancellationToken) != null;
    }

    private async Task<bool> MotorcycleNotInUse(Guid id, CancellationToken cancellationToken)
    {
        return !await _rentalRepository.ExistsActiveRentalForMotorcycleAsync(id, cancellationToken);
    }

    private async Task<bool> DeliveryPersonExists(Guid id, CancellationToken cancellationToken)
    {
        return await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken) != null;
    }

    private async Task<bool> DeliveryPersonCanRentMotorcycle(Guid id, CancellationToken cancellationToken)
    {
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        if (deliveryPerson == null)
            return false;

        return deliveryPerson.CanRentMotorcycle();
    }

    private async Task<bool> DeliveryPersonNotAlreadyRenting(Guid id, CancellationToken cancellationToken)
    {
        var activeRental = await _rentalRepository.GetActiveRentalByDeliveryPersonIdAsync(id, cancellationToken);
        return activeRental == null;
    }

    private bool BeAtLeastTomorrow(DateOnly date)
    {
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        return date >= tomorrow;
    }
}