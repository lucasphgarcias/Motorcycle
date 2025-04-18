using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Validators;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Interfaces.Repositories;
using System;
using Xunit;

namespace Motorcycle.Application.Tests.Validators;

public class CreateRentalDtoValidatorTests
{
    private readonly CreateRentalDtoValidator _validator;
    private readonly Mock<IMotorcycleRepository> _mockMotorcycleRepository;
    private readonly Mock<IDeliveryPersonRepository> _mockDeliveryPersonRepository;
    private readonly Mock<IRentalRepository> _mockRentalRepository;

    public CreateRentalDtoValidatorTests()
    {
        _mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
        _mockDeliveryPersonRepository = new Mock<IDeliveryPersonRepository>();
        _mockRentalRepository = new Mock<IRentalRepository>();
        
        _validator = new CreateRentalDtoValidator(
            _mockMotorcycleRepository.Object,
            _mockDeliveryPersonRepository.Object,
            _mockRentalRepository.Object);
    }

    [Fact]
    public void Validator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        
        var dto = new CreateRentalDto
        {
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            StartDate = tomorrow,
            PlanType = RentalPlanType.SevenDays
        };

        _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(motorcycleId, default))
            .ReturnsAsync(new MotorcycleEntity());
        
        _mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPersonId, default))
            .ReturnsAsync(new DeliveryPersonEntity());
        
        _mockRentalRepository.Setup(repo => repo.ExistsActiveRentalForMotorcycleAsync(motorcycleId, default))
            .ReturnsAsync(false);
        
        _mockRentalRepository.Setup(repo => repo.GetActiveRentalByDeliveryPersonIdAsync(deliveryPersonId, default))
            .ReturnsAsync((RentalEntity)null);
        
        var deliveryPerson = new Mock<DeliveryPersonEntity>();
        deliveryPerson.Setup(dp => dp.CanRentMotorcycle()).Returns(true);
        _mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPersonId, default))
            .ReturnsAsync(deliveryPerson.Object);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void MotorcycleId_WithEmptyGuid_ShouldHaveError()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var dto = new CreateRentalDto
        {
            MotorcycleId = Guid.Empty,
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = tomorrow,
            PlanType = RentalPlanType.SevenDays
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MotorcycleId);
    }

    [Fact]
    public void DeliveryPersonId_WithEmptyGuid_ShouldHaveError()
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var dto = new CreateRentalDto
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.Empty,
            StartDate = tomorrow,
            PlanType = RentalPlanType.SevenDays
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeliveryPersonId);
    }

    [Fact]
    public void StartDate_WithPastDate_ShouldHaveError()
    {
        // Arrange
        var pastDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10));
        var dto = new CreateRentalDto
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = pastDate,
            PlanType = RentalPlanType.SevenDays
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Theory]
    [InlineData(0)] // Valor inválido para o enum
    public void PlanType_WithInvalidValue_ShouldHaveError(int planType)
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var dto = new CreateRentalDto
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = tomorrow,
            PlanType = (RentalPlanType)planType
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PlanType);
    }

    [Theory]
    [InlineData(RentalPlanType.SevenDays)]
    [InlineData(RentalPlanType.FifteenDays)]
    [InlineData(RentalPlanType.ThirtyDays)]
    [InlineData(RentalPlanType.FortyFiveDays)]
    public void PlanType_WithStandardPeriods_ShouldNotHaveError(RentalPlanType planType)
    {
        // Arrange
        var tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        
        var dto = new CreateRentalDto
        {
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            StartDate = tomorrow,
            PlanType = planType
        };

        _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(motorcycleId, default))
            .ReturnsAsync(new MotorcycleEntity());
        
        _mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPersonId, default))
            .ReturnsAsync(new DeliveryPersonEntity());
        
        _mockRentalRepository.Setup(repo => repo.ExistsActiveRentalForMotorcycleAsync(motorcycleId, default))
            .ReturnsAsync(false);
        
        _mockRentalRepository.Setup(repo => repo.GetActiveRentalByDeliveryPersonIdAsync(deliveryPersonId, default))
            .ReturnsAsync((RentalEntity)null);
        
        var deliveryPerson = new Mock<DeliveryPersonEntity>();
        deliveryPerson.Setup(dp => dp.CanRentMotorcycle()).Returns(true);
        _mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPersonId, default))
            .ReturnsAsync(deliveryPerson.Object);

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PlanType);
    }
} 