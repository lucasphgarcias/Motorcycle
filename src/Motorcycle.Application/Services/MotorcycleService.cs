using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;

namespace Motorcycle.Application.Services;

public class MotorcycleService : IMotorcycleService
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IEventPublisher _eventPublisher;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateMotorcycleDto> _createValidator;
    private readonly IValidator<UpdateMotorcycleLicensePlateDto> _updateValidator;
    private readonly ILogger<MotorcycleService> _logger;

    public MotorcycleService(
        IMotorcycleRepository motorcycleRepository,
        IEventPublisher eventPublisher,
        IMapper mapper,
        IValidator<CreateMotorcycleDto> createValidator,
        IValidator<UpdateMotorcycleLicensePlateDto> updateValidator,
        ILogger<MotorcycleService> logger)
    {
        _motorcycleRepository = motorcycleRepository;
        _eventPublisher = eventPublisher;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<MotorcycleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all motorcycles");
        var motorcycles = await _motorcycleRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<MotorcycleDto>>(motorcycles);
    }

    public async Task<MotorcycleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting motorcycle with ID: {MotorcycleId}", id);
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        return motorcycle != null ? _mapper.Map<MotorcycleDto>(motorcycle) : null;
    }

    public async Task<IEnumerable<MotorcycleDto>> SearchByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching motorcycles by license plate: {LicensePlate}", licensePlate);
        var motorcycles = await _motorcycleRepository.SearchByLicensePlateAsync(licensePlate, cancellationToken);
        return _mapper.Map<IEnumerable<MotorcycleDto>>(motorcycles);
    }

    public async Task<MotorcycleDto> CreateAsync(CreateMotorcycleDto createDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new motorcycle with license plate: {LicensePlate}", createDto.LicensePlate);
        
        var validationResult = await _createValidator.ValidateAsync(createDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var motorcycle = _mapper.Map<MotorcycleEntity>(createDto);
        
        await _motorcycleRepository.AddAsync(motorcycle, cancellationToken);
        
        // Publicar eventos
        foreach (var domainEvent in motorcycle.DomainEvents)
        {
            await _eventPublisher.PublishAsync(domainEvent, cancellationToken);
        }
        
        motorcycle.ClearDomainEvents();
        
        return _mapper.Map<MotorcycleDto>(motorcycle);
    }

    public async Task<MotorcycleDto> UpdateAsync(Guid id, CreateMotorcycleDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating motorcycle with ID: {MotorcycleId}", id);
        
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle == null)
            throw new DomainException($"Moto com ID {id} não encontrada.");

        var validationResult = await _createValidator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Criar uma nova entidade com os novos valores
        var updatedMotorcycle = _mapper.Map<MotorcycleEntity>(updateDto);
        
        // Copiar o ID original
        var motorcycleWithOriginalId = MotorcycleEntity.Create(
            updatedMotorcycle.Model,
            updatedMotorcycle.Year,
            updatedMotorcycle.LicensePlate.Value);
        
        await _motorcycleRepository.UpdateAsync(motorcycleWithOriginalId, cancellationToken);
        
        return _mapper.Map<MotorcycleDto>(motorcycleWithOriginalId);
    }

    public async Task<MotorcycleDto> UpdateLicensePlateAsync(Guid id, UpdateMotorcycleLicensePlateDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating license plate for motorcycle with ID: {MotorcycleId}", id);
        
        var validationResult = await _updateValidator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle == null)
            throw new DomainException($"Moto com ID {id} não encontrada.");

        motorcycle.UpdateLicensePlate(updateDto.LicensePlate);
        
        await _motorcycleRepository.UpdateAsync(motorcycle, cancellationToken);
        
        return _mapper.Map<MotorcycleDto>(motorcycle);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting motorcycle with ID: {MotorcycleId}", id);
        
        var motorcycle = await _motorcycleRepository.GetByIdAsync(id, cancellationToken);
        if (motorcycle == null)
            return false;

        if (!motorcycle.CanBeRemoved())
            throw new DomainException("Não é possível remover uma moto que possui locações registradas.");

        await _motorcycleRepository.RemoveAsync(motorcycle, cancellationToken);
        
        return true;
    }
}