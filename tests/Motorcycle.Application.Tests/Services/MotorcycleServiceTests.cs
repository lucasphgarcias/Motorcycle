using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Interfaces;
using Motorcycle.Application.Services;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Events;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;
using Xunit;
using FluentAssertions;

namespace Motorcycle.Application.Tests.Services;

public class MotorcycleServiceTests
{
    private readonly Mock<IMotorcycleRepository> _mockRepository;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateMotorcycleDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateMotorcycleLicensePlateDto>> _mockUpdateValidator;
    private readonly Mock<ILogger<MotorcycleService>> _mockLogger;
    private readonly MotorcycleService _service;

    public MotorcycleServiceTests()
    {
        _mockRepository = new Mock<IMotorcycleRepository>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateMotorcycleDto>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateMotorcycleLicensePlateDto>>();
        _mockLogger = new Mock<ILogger<MotorcycleService>>();

        _service = new MotorcycleService(
            _mockRepository.Object,
            _mockEventPublisher.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockUpdateValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var motorcycles = new List<MotorcycleEntity> { CreateSampleMotorcycle(), CreateSampleMotorcycle() };
        var dtos = new List<MotorcycleDto> { new MotorcycleDto(), new MotorcycleDto() };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycles);
        _mockMapper.Setup(m => m.Map<IEnumerable<MotorcycleDto>>(motorcycles))
            .Returns(dtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeSameAs(dtos);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<MotorcycleDto>>(motorcycles), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnMappedDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = CreateSampleMotorcycle();
        var dto = new MotorcycleDto();

        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        _mockMapper.Setup(m => m.Map<MotorcycleDto>(motorcycle))
            .Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeSameAs(dto);
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<MotorcycleDto>(motorcycle), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MotorcycleEntity)null!);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<MotorcycleDto>(It.IsAny<MotorcycleEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateAndReturnMotorcycle()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto 
        { 
            Model = "Honda CG 160", 
            Year = 2023, 
            LicensePlate = "ABC1234" 
        };
        var validationResult = new ValidationResult();
        var motorcycle = CreateSampleMotorcycle();
        var dto = new MotorcycleDto();

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<MotorcycleEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<MotorcycleDto>(It.IsAny<MotorcycleEntity>()))
            .Returns(dto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeSameAs(dto);
        _mockCreateValidator.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MotorcycleEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockEventPublisher.Verify(e => e.PublishAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDto_ShouldThrowDomainException()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto();
        var validationFailures = new List<ValidationFailure> 
        {
            new ValidationFailure("Model", "O modelo é obrigatório"),
            new ValidationFailure("LicensePlate", "A placa é obrigatória")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.CreateAsync(createDto);
        await action.Should().ThrowAsync<DomainException>();
        
        _mockCreateValidator.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<MotorcycleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithMotorcycleHavingNoRentals_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = CreateSampleMotorcycle();

        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        _mockRepository.Setup(r => r.RemoveAsync(motorcycle, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.RemoveAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MotorcycleEntity)null!);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.RemoveAsync(It.IsAny<MotorcycleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithMotorcycleHavingRentals_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = CreateSampleMotorcycle();
        
        // Simular que a moto tem aluguéis
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        
        // Simular que a moto não pode ser removida
        var motorcycleMock = new Mock<MotorcycleEntity>();
        motorcycleMock.Setup(m => m.CanBeRemoved()).Returns(false);
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycleMock.Object);

        // Act & Assert
        await _service.Invoking(s => s.DeleteAsync(id))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Não é possível remover uma moto que possui locações registradas.");
    }

    [Fact]
    public async Task SearchByLicensePlateAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var licensePlate = "ABC1234";
        var motorcycles = new List<MotorcycleEntity> { CreateSampleMotorcycle(), CreateSampleMotorcycle() };
        var dtos = new List<MotorcycleDto> { new MotorcycleDto(), new MotorcycleDto() };

        _mockRepository.Setup(r => r.SearchByLicensePlateAsync(licensePlate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycles);
        _mockMapper.Setup(m => m.Map<IEnumerable<MotorcycleDto>>(motorcycles))
            .Returns(dtos);

        // Act
        var result = await _service.SearchByLicensePlateAsync(licensePlate);

        // Assert
        result.Should().BeSameAs(dtos);
        _mockRepository.Verify(r => r.SearchByLicensePlateAsync(licensePlate, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<MotorcycleDto>>(motorcycles), Times.Once);
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_WithValidDto_ShouldUpdateAndReturnMotorcycle()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateMotorcycleLicensePlateDto { LicensePlate = "XYZ5678" };
        var validationResult = new ValidationResult();
        var motorcycle = CreateSampleMotorcycle();
        var dto = new MotorcycleDto();

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        _mockRepository.Setup(r => r.UpdateAsync(motorcycle, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<MotorcycleDto>(motorcycle))
            .Returns(dto);

        // Act
        var result = await _service.UpdateLicensePlateAsync(id, updateDto);

        // Assert
        result.Should().BeSameAs(dto);
        _mockUpdateValidator.Verify(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(motorcycle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_WithInvalidDto_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateMotorcycleLicensePlateDto { LicensePlate = "INVALID" };
        var validationFailures = new List<ValidationFailure> 
        {
            new ValidationFailure("LicensePlate", "Formato de placa inválido")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.UpdateLicensePlateAsync(id, updateDto);
        await action.Should().ThrowAsync<DomainException>();
        
        _mockUpdateValidator.Verify(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<MotorcycleEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateLicensePlateAsync_WithNonExistingId_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new UpdateMotorcycleLicensePlateDto { LicensePlate = "XYZ5678" };
        var validationResult = new ValidationResult();

        _mockUpdateValidator.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((MotorcycleEntity)null!);

        // Act & Assert
        var action = () => _service.UpdateLicensePlateAsync(id, updateDto);
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage($"Moto com ID {id} não encontrada.");
    }

    private MotorcycleEntity CreateSampleMotorcycle()
    {
        return MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
    }
} 