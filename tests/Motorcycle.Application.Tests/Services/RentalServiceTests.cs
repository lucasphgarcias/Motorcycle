using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Services;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.ValueObjects;
using System;
using Xunit;
using FluentAssertions;

namespace Motorcycle.Application.Tests.Services;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _mockRentalRepository;
    private readonly Mock<IMotorcycleRepository> _mockMotorcycleRepository;
    private readonly Mock<IDeliveryPersonRepository> _mockDeliveryPersonRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateRentalDto>> _mockCreateValidator;
    private readonly Mock<IValidator<ReturnMotorcycleDto>> _mockReturnValidator;
    private readonly Mock<ILogger<RentalService>> _mockLogger;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _mockRentalRepository = new Mock<IRentalRepository>();
        _mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
        _mockDeliveryPersonRepository = new Mock<IDeliveryPersonRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateRentalDto>>();
        _mockReturnValidator = new Mock<IValidator<ReturnMotorcycleDto>>();
        _mockLogger = new Mock<ILogger<RentalService>>();

        _service = new RentalService(
            _mockRentalRepository.Object,
            _mockMotorcycleRepository.Object,
            _mockDeliveryPersonRepository.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockReturnValidator.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var rentals = new List<RentalEntity> { CreateSampleRental(), CreateSampleRental() };
        var dtos = new List<RentalDto> { new(), new() };

        _mockRentalRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentals);
        _mockMapper.Setup(m => m.Map<IEnumerable<RentalDto>>(rentals))
            .Returns(dtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeSameAs(dtos);
        _mockRentalRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<RentalDto>>(rentals), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnMappedDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rental = CreateSampleRental();
        var dto = new RentalDto();

        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);
        _mockMapper.Setup(m => m.Map<RentalDto>(rental))
            .Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeSameAs(dto);
        _mockRentalRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<RentalDto>(rental), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RentalEntity)null!);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _mockRentalRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<RentalDto>(It.IsAny<RentalEntity>()), Times.Never);
    }

    [Fact]
    public async Task GetByMotorcycleIdAsync_ShouldReturnMappedDtos()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var rentals = new List<RentalEntity> { CreateSampleRental(), CreateSampleRental() };
        var dtos = new List<RentalDto> { new(), new() };

        _mockRentalRepository.Setup(r => r.GetByMotorcycleIdAsync(motorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rentals);
        _mockMapper.Setup(m => m.Map<IEnumerable<RentalDto>>(rentals))
            .Returns(dtos);

        // Act
        var result = await _service.GetByMotorcycleIdAsync(motorcycleId);

        // Assert
        result.Should().BeSameAs(dtos);
        _mockRentalRepository.Verify(r => r.GetByMotorcycleIdAsync(motorcycleId, It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<RentalDto>>(rentals), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateAndReturnRental()
    {
        // Arrange
        var createDto = new CreateRentalDto 
        { 
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), // Dia seguinte ao atual
            PlanType = RentalPlanType.SevenDays
        };
        var validationResult = new ValidationResult();
        var motorcycle = CreateSampleMotorcycle();
        var deliveryPerson = CreateSampleDeliveryPerson();
        var dto = new RentalDto();

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockMotorcycleRepository.Setup(r => r.GetByIdAsync(createDto.MotorcycleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);
        _mockDeliveryPersonRepository.Setup(r => r.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);
        _mockRentalRepository.Setup(r => r.AddAsync(It.IsAny<RentalEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<RentalDto>(It.IsAny<RentalEntity>()))
            .Returns(dto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeSameAs(dto);
        _mockCreateValidator.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        _mockMotorcycleRepository.Verify(r => r.GetByIdAsync(createDto.MotorcycleId, It.IsAny<CancellationToken>()), Times.Once);
        _mockDeliveryPersonRepository.Verify(r => r.GetByIdAsync(createDto.DeliveryPersonId, It.IsAny<CancellationToken>()), Times.Once);
        _mockRentalRepository.Verify(r => r.AddAsync(It.IsAny<RentalEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidDto_ShouldThrowDomainException()
    {
        // Arrange
        var createDto = new CreateRentalDto();
        var validationFailures = new List<ValidationFailure> 
        {
            new ValidationFailure("StartDate", "A data de início é obrigatória"),
            new ValidationFailure("PlanType", "O tipo de plano é obrigatório")
        };
        var validationResult = new ValidationResult(validationFailures);

        _mockCreateValidator.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        var action = () => _service.CreateAsync(createDto);
        await action.Should().ThrowAsync<DomainException>();
        
        _mockCreateValidator.Verify(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        _mockRentalRepository.Verify(r => r.AddAsync(It.IsAny<RentalEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateDto = new CreateRentalDto();

        // Act & Assert
        var action = () => _service.UpdateAsync(id, updateDto);
        await action.Should().ThrowAsync<DomainException>()
            .WithMessage("A atualização de aluguéis não é permitida. Para finalizar um aluguel, utilize o método de devolução.");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingRental_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rental = CreateSampleRental();

        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rental);
        _mockRentalRepository.Setup(r => r.RemoveAsync(rental, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _mockRentalRepository.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mockRentalRepository.Verify(r => r.RemoveAsync(rental, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingRental_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync((RentalEntity)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _mockRentalRepository.Verify(r => r.RemoveAsync(It.IsAny<RentalEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CalculateTotalAmountAsync_WithValidData_ShouldReturnTotalAmountDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
        
        var rental = CreateSampleRental();
        var motorcycle = CreateSampleMotorcycle();
        var deliveryPerson = CreateSampleDeliveryPerson();
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync(rental);
        _mockMotorcycleRepository.Setup(r => r.GetByIdAsync(rental.MotorcycleId, default))
            .ReturnsAsync(motorcycle);
        _mockDeliveryPersonRepository.Setup(r => r.GetByIdAsync(rental.DeliveryPersonId, default))
            .ReturnsAsync(deliveryPerson);
        
        // Act
        var result = await _service.CalculateTotalAmountAsync(id, returnDate);
        
        // Assert
        result.Should().NotBeNull();
        result.RentalId.Should().Be(rental.Id);
        result.TotalAmount.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task CalculateTotalAmountAsync_WithNonExistingRental_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10));
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync((RentalEntity)null);
        
        // Act & Assert
        await _service.Invoking(s => s.CalculateTotalAmountAsync(id, returnDate))
            .Should().ThrowAsync<DomainException>()
            .WithMessage($"Aluguel com ID {id} não encontrado.");
    }
    
    [Fact]
    public async Task CalculateTotalAmountAsync_WithLateReturn_ShouldIncludeExtraAmount()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Simular uma devolução tardia (30 dias para um plano de 7 dias)
        var returnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
        
        var rental = CreateSampleRental();
        var motorcycle = CreateSampleMotorcycle();
        var deliveryPerson = CreateSampleDeliveryPerson();
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync(rental);
        _mockMotorcycleRepository.Setup(r => r.GetByIdAsync(rental.MotorcycleId, default))
            .ReturnsAsync(motorcycle);
        _mockDeliveryPersonRepository.Setup(r => r.GetByIdAsync(rental.DeliveryPersonId, default))
            .ReturnsAsync(deliveryPerson);
        
        // Act
        var result = await _service.CalculateTotalAmountAsync(id, returnDate);
        
        // Assert
        result.Should().NotBeNull();
        result.IsLateReturn.Should().BeTrue();
        result.ExtraDays.Should().HaveValue();
        result.ExtraAmount.Should().HaveValue();
        result.ExtraAmount.Value.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public async Task CalculateTotalAmountAsync_WithEarlyReturn_ShouldIncludePenaltyAmount()
    {
        // Arrange
        var id = Guid.NewGuid();
        // Simular uma devolução antecipada (3 dias para um plano de 7 dias)
        var returnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
        
        var rental = CreateSampleRental();
        var motorcycle = CreateSampleMotorcycle();
        var deliveryPerson = CreateSampleDeliveryPerson();
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync(rental);
        _mockMotorcycleRepository.Setup(r => r.GetByIdAsync(rental.MotorcycleId, default))
            .ReturnsAsync(motorcycle);
        _mockDeliveryPersonRepository.Setup(r => r.GetByIdAsync(rental.DeliveryPersonId, default))
            .ReturnsAsync(deliveryPerson);
        
        // Act
        var result = await _service.CalculateTotalAmountAsync(id, returnDate);
        
        // Assert
        result.Should().NotBeNull();
        result.IsEarlyReturn.Should().BeTrue();
        result.UnusedDays.Should().HaveValue();
        result.PenaltyAmount.Should().HaveValue();
        result.PenaltyAmount.Value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ReturnMotorcycleAsync_WithValidData_ShouldReturnTotalAmountDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
        };
        
        var rental = CreateSampleRental();
        var motorcycle = CreateSampleMotorcycle();
        var deliveryPerson = CreateSampleDeliveryPerson();
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync(rental);
        _mockRentalRepository.Setup(r => r.UpdateAsync(It.IsAny<RentalEntity>(), default))
            .Returns(Task.CompletedTask);
        _mockMotorcycleRepository.Setup(r => r.GetByIdAsync(rental.MotorcycleId, default))
            .ReturnsAsync(motorcycle);
        _mockDeliveryPersonRepository.Setup(r => r.GetByIdAsync(rental.DeliveryPersonId, default))
            .ReturnsAsync(deliveryPerson);
        _mockReturnValidator.Setup(v => v.ValidateAsync(returnDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        // Act
        var result = await _service.ReturnMotorcycleAsync(id, returnDto);
        
        // Assert
        result.Should().NotBeNull();
        result.RentalId.Should().Be(rental.Id);
        result.TotalAmount.Should().BeGreaterThan(0);
        _mockRentalRepository.Verify(r => r.UpdateAsync(It.IsAny<RentalEntity>(), default), Times.Once);
    }
    
    [Fact]
    public async Task ReturnMotorcycleAsync_WithInvalidDto_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
        };
        
        var validationResult = new FluentValidation.Results.ValidationResult(
            new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("ReturnDate", "A data de devolução é inválida")
            });
        
        _mockReturnValidator.Setup(v => v.ValidateAsync(returnDto, default))
            .ReturnsAsync(validationResult);
        
        // Act & Assert
        await _service.Invoking(s => s.ReturnMotorcycleAsync(id, returnDto))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("A data de devolução é inválida");
    }
    
    [Fact]
    public async Task ReturnMotorcycleAsync_WithNonExistingRental_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
        };
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync((RentalEntity)null);
        _mockReturnValidator.Setup(v => v.ValidateAsync(returnDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        // Act & Assert
        await _service.Invoking(s => s.ReturnMotorcycleAsync(id, returnDto))
            .Should().ThrowAsync<DomainException>()
            .WithMessage($"Aluguel com ID {id} não encontrado.");
    }
    
    [Fact]
    public async Task ReturnMotorcycleAsync_WithAlreadyFinishedRental_ShouldThrowDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDto = new ReturnMotorcycleDto
        {
            ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10))
        };
        
        var rental = CreateSampleRental();
        
        // Simular um aluguel já finalizado
        rental.ReturnMotorcycle(DateOnly.FromDateTime(DateTime.Today.AddDays(5)));
        
        _mockRentalRepository.Setup(r => r.GetByIdAsync(id, default))
            .ReturnsAsync(rental);
        _mockReturnValidator.Setup(v => v.ValidateAsync(returnDto, default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        
        // Act & Assert
        await _service.Invoking(s => s.ReturnMotorcycleAsync(id, returnDto))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("Este aluguel já foi finalizado.");
    }

    private MotorcycleEntity CreateSampleMotorcycle()
    {
        return MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
    }

    private DeliveryPersonEntity CreateSampleDeliveryPerson()
    {
        // Criar uma instância válida de DeliveryPersonEntity usando o método Create
        return DeliveryPersonEntity.Create(
            name: "João da Silva", 
            cnpj: "04.252.011/0001-10", // CNPJ válido para testes
            birthDate: DateOnly.FromDateTime(DateTime.Today.AddYears(-30)), 
            licenseNumber: "12345678901", 
            licenseType: LicenseType.A
        );
    }

    private RentalEntity CreateSampleRental()
    {
        // Usar o método estático Create para criar uma instância de RentalEntity
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)); // Dia seguinte ao atual
        var planType = RentalPlanType.SevenDays;
        var deliveryPerson = CreateSampleDeliveryPerson();

        // Estamos usando reflected apenas para testes - em produção usaríamos o método Create
        var rental = RentalEntity.Create(
            motorcycleId, 
            deliveryPersonId, 
            startDate, 
            planType, 
            deliveryPerson
        );

        return rental;
    }
} 