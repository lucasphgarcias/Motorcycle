using Microsoft.AspNetCore.Http;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;


namespace Motorcycle.Application.Services;

public class DeliveryPersonService : IDeliveryPersonService
{
    private readonly IDeliveryPersonRepository _deliveryPersonRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateDeliveryPersonDto> _createValidator;
    private readonly IValidator<UpdateDriverLicenseImageDto> _imageValidator;
    private readonly ILogger<DeliveryPersonService> _logger;

    public DeliveryPersonService(
        IDeliveryPersonRepository deliveryPersonRepository,
        IFileStorageService fileStorageService,
        IMapper mapper,
        IValidator<CreateDeliveryPersonDto> createValidator,
        IValidator<UpdateDriverLicenseImageDto> imageValidator,
        ILogger<DeliveryPersonService> logger)
    {
        _deliveryPersonRepository = deliveryPersonRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _createValidator = createValidator;
        _imageValidator = imageValidator;
        _logger = logger;
    }

    public async Task<IEnumerable<DeliveryPersonDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all delivery persons");
        var deliveryPersons = await _deliveryPersonRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<DeliveryPersonDto>>(deliveryPersons);
    }

    public async Task<DeliveryPersonDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting delivery person with ID: {DeliveryPersonId}", id);
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        return deliveryPerson != null ? _mapper.Map<DeliveryPersonDto>(deliveryPerson) : null;
    }

    public async Task<DeliveryPersonDto?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting delivery person by CNPJ: {Cnpj}", cnpj);
        var deliveryPerson = await _deliveryPersonRepository.GetByCnpjAsync(cnpj, cancellationToken);
        return deliveryPerson != null ? _mapper.Map<DeliveryPersonDto>(deliveryPerson) : null;
    }

    public async Task<DeliveryPersonDto?> GetByDriverLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting delivery person by license number: {LicenseNumber}", licenseNumber);
        var deliveryPerson = await _deliveryPersonRepository.GetByDriverLicenseNumberAsync(licenseNumber, cancellationToken);
        return deliveryPerson != null ? _mapper.Map<DeliveryPersonDto>(deliveryPerson) : null;
    }

    public async Task<DeliveryPersonDto> CreateAsync(CreateDeliveryPersonDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new delivery person with CNPJ: {Cnpj}", createDto?.Cnpj);
            
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto), "Os dados do entregador são obrigatórios.");
            
            if (_createValidator == null)
                throw new InvalidOperationException("O validador não foi inicializado corretamente.");
            
            var validationResult = await _createValidator.ValidateAsync(createDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var deliveryPerson = _mapper.Map<DeliveryPersonEntity>(createDto);
            
            await _deliveryPersonRepository.AddAsync(deliveryPerson, cancellationToken);
            
            return _mapper.Map<DeliveryPersonDto>(deliveryPerson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar entregador: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    public async Task<DeliveryPersonDto> UpdateAsync(Guid id, CreateDeliveryPersonDto updateDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating delivery person with ID: {DeliveryPersonId}", id);
        
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        if (deliveryPerson == null)
            throw new DomainException($"Entregador com ID {id} não encontrado.");

        var validationResult = await _createValidator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Criar uma nova entidade com os novos valores
        var updatedDeliveryPerson = _mapper.Map<DeliveryPersonEntity>(updateDto);
        
        // Manter o ID original
        var reflection = typeof(Entity).GetProperty("Id");
        reflection?.SetValue(updatedDeliveryPerson, id);
        
        await _deliveryPersonRepository.UpdateAsync(updatedDeliveryPerson, cancellationToken);
        
        return _mapper.Map<DeliveryPersonDto>(updatedDeliveryPerson);
    }

    public async Task<DeliveryPersonDto> UpdateDriverLicenseImageAsync(Guid id, IFormFile image, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating driver license image for delivery person with ID: {DeliveryPersonId}", id);
        
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        if (deliveryPerson == null)
            throw new DomainException($"Entregador com ID {id} não encontrado.");

        var updateDto = new UpdateDriverLicenseImageDto { LicenseImage = image };
        var validationResult = await _imageValidator.ValidateAsync(updateDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new DomainException(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        // Gerar um nome único para o arquivo
        var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        
        // Fazer upload do arquivo
        await using var stream = image.OpenReadStream();
        var imagePath = await _fileStorageService.UploadFileAsync(stream, fileName, image.ContentType, cancellationToken);
        
        // Atualizar o caminho da imagem da CNH
        deliveryPerson.UpdateDriverLicenseImage(imagePath);
        
        await _deliveryPersonRepository.UpdateAsync(deliveryPerson, cancellationToken);
        
        return _mapper.Map<DeliveryPersonDto>(deliveryPerson);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting delivery person with ID: {DeliveryPersonId}", id);
        
        var deliveryPerson = await _deliveryPersonRepository.GetByIdAsync(id, cancellationToken);
        if (deliveryPerson == null)
            return false;

        await _deliveryPersonRepository.RemoveAsync(deliveryPerson, cancellationToken);
        
        return true;
    }
}