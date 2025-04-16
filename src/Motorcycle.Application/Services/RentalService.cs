using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.ValueObjects;

namespace Motorcycle.Application.Services;

public class RentalService : IRentalService
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateRentalDto> _createValidator;
    private readonly IValidator<ReturnMotorcycleDto> _returnValidator;
    private readonly ILogger<RentalService> _logger;

    public RentalService(
        IRentalRepository rentalRepository,
        IMotorcycleRepository motorcycleRepository,
        IDeliveryPersonRepository deliveryPersonRepository,
        IMapper mapper,
        IValidator<CreateRentalDto> createValidator,
        IValidator<ReturnMotorcycleDto> returnValidator,
        ILogger<RentalService> logger)
    {
        _rentalRepository = rentalRepository;
        _motorcycleRepository = motorcycleRepository;
        _deliveryPersonRepository = deliveryPersonRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _returnValidator = returnValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<RentalDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all rentals");
        var rentals = await _rentalRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<RentalDto>>(rentals);
    }

    public async Task<RentalDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting rental with ID: {RentalId}", id);
        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        return rental != null ? _mapper.Map<RentalDto>(rental) : null;
    }

    public async Task<IEnumerable<RentalDto>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting rentals for motorcycle with ID: {MotorcycleId}", motorcycleId);
        var rentals = await _rentalRepository.GetByMotorcycleIdAsync(motorcycleId, cancellationToken);
        return _mapper.Map<IEnumerable<RentalDto>>(rentals);
    }

    public async Task<IEnumerable<RentalDto>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting rentals for delivery person with ID: {DeliveryPersonId}", deliveryPersonId);
        var rentals = await _rentalRepository.GetByDeliveryPersonIdAsync(deliveryPersonId, cancellationToken);
        return _mapper.Map<IEnumerable<RentalDto>>(rentals);
    }

    public async Task<RentalDto?> GetActiveRentalByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting active rental for delivery person with ID: {DeliveryPersonId}", deliveryPersonId);
        var rental = await _rentalRepository.GetActiveRentalByDeliveryPersonIdAsync(deliveryPersonId, cancellationToken);
        return rental != null ? _mapper.Map<RentalDto>(rental) : null;
    }

    public async Task<RentalDto> CreateAsync(CreateRentalDto createDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new rental for motorcycle ID: {MotorcycleId} and delivery person ID: {DeliveryPersonId}",
            createDto.MotorcycleId, createDto.DeliveryPersonId);
        
        var validationResult = await _createValidator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var motorcycle = await _motorcycleRepository.GetByIdAsync(createDto.MotorcycleId, cancellationToken);
        if (motorcycle == null)
            throw new DomainException($"Moto com ID {createDto.MotorcycleId} não encontrada.");

        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(createDto.DeliveryPersonId, cancellationToken);
        if (deliveryPerson == null)
            throw new DomainException($"Entregador com ID {createDto.DeliveryPersonId} não encontrado.");

        var rental = RentalEntity.Create(
            createDto.MotorcycleId,
            createDto.DeliveryPersonId,
            createDto.StartDate,
            createDto.PlanType,
            deliveryPerson);
        
        await _rentalRepository.AddAsync(rental, cancellationToken);
        
        return _mapper.Map<RentalDto>(rental);
    }

    public async Task<RentalDto> UpdateAsync(Guid id, CreateRentalDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating rental with ID: {RentalId}", id);
        
        throw new DomainException("A atualização de aluguéis não é permitida. Para finalizar um aluguel, utilize o método de devolução.");
    }

    public async Task<RentalTotalAmountDto> ReturnMotorcycleAsync(Guid id, ReturnMotorcycleDto returnDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing motorcycle return for rental ID: {RentalId}", id);
        
        var validationResult = await _returnValidator.ValidateAsync(returnDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        if (rental == null)
            throw new DomainException($"Aluguel com ID {id} não encontrado.");

        // Verificar se o aluguel já foi finalizado
        if (rental.TotalAmount != null)
            throw new DomainException("Este aluguel já foi finalizado.");

        // Registrar a devolução
        rental.ReturnMotorcycle(returnDto.ReturnDate);
        
        await _rentalRepository.UpdateAsync(rental, cancellationToken);
        
        // Calcular e retornar os detalhes do valor total
        return await CalculateTotalAmountDetailsAsync(rental, cancellationToken);
    }

    public async Task<RentalTotalAmountDto> CalculateTotalAmountAsync(Guid id, DateOnly returnDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating total amount for rental ID: {RentalId} with return date: {ReturnDate}", id, returnDate);
        
        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        if (rental == null)
            throw new DomainException($"Aluguel com ID {id} não encontrado.");

        // Criar uma cópia do aluguel para simulação
        var simulatedRental = await CloneRentalAsync(rental, cancellationToken);
        
        // Simular a devolução na data informada
        simulatedRental.ReturnMotorcycle(returnDate);
        
        // Calcular e retornar os detalhes do valor total
        return await CalculateTotalAmountDetailsAsync(simulatedRental, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting rental with ID: {RentalId}", id);
        
        var rental = await _rentalRepository.GetByIdAsync(id, cancellationToken);
        if (rental == null)
            return false;

        await _rentalRepository.RemoveAsync(rental, cancellationToken);
        
        return true;
    }

    private async Task<RentalEntity> CloneRentalAsync(RentalEntity original, CancellationToken cancellationToken)
    {
        // Obter os objetos relacionados
        var motorcycle = await _motorcycleRepository.GetByIdAsync(original.MotorcycleId, cancellationToken);
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(original.DeliveryPersonId, cancellationToken);
        
        if (motorcycle == null || deliveryPerson == null)
            throw new DomainException("Não foi possível clonar o aluguel devido a dados faltantes.");

        // Criar um novo aluguel com os mesmos dados
        var clone = RentalEntity.Create(
            original.MotorcycleId,
            original.DeliveryPersonId,
            original.Period.StartDate,
            original.Period.PlanType,
            deliveryPerson);
        
        return clone;
    }

    private async Task<RentalTotalAmountDto> CalculateTotalAmountDetailsAsync(RentalEntity rental, CancellationToken cancellationToken)
    {
        if (rental.Period.ActualEndDate == null || rental.TotalAmount == null)
            throw new DomainException("O aluguel não possui informações de devolução.");

        var period = rental.Period;
        var totalAmount = rental.TotalAmount.Amount;
        
        var result = new RentalTotalAmountDto
        {
            RentalId = rental.Id,
            TotalAmount = totalAmount,
            ActualDays = period.CalculateRentalDays(),
            IsEarlyReturn = period.IsEarlyReturn(),
            IsLateReturn = period.IsLateReturn()
        };

        if (period.IsEarlyReturn())
        {
            result.UnusedDays = period.CalculateUnusedDays();
            
            // Calcular o valor da multa (simplificado)
            var unusedAmount = rental.DailyRate.Amount * result.UnusedDays.Value;
            var penaltyPercentage = GetEarlyReturnPenaltyPercentage(period.PlanType);
            result.PenaltyAmount = unusedAmount * penaltyPercentage;
        }
        else if (period.IsLateReturn())
        {
            result.ExtraDays = period.CalculateExtraDays();
            
            // Calcular o valor adicional (taxa fixa de R$50 por dia adicional)
            result.ExtraAmount = result.ExtraDays.Value * 50m;
        }

        return result;
    }

    private static decimal GetEarlyReturnPenaltyPercentage(RentalPlanType planType)
    {
        return planType switch
        {
            RentalPlanType.SevenDays => 0.2m, // 20%
            RentalPlanType.FifteenDays => 0.4m, // 40%
            _ => 0m // Outros planos não têm multa
        };
    }
}