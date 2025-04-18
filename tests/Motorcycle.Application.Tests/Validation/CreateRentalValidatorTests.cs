using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Validators;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Interfaces.Repositories;
using Xunit;

namespace Motorcycle.Application.Tests.Validation;

public class CreateRentalValidatorTests
{
    private readonly CreateRentalDtoValidator _validator;
    private readonly Mock<IMotorcycleRepository> _mockMotorcycleRepository;
    private readonly Mock<IDeliveryPersonRepository> _mockDeliveryPersonRepository;
    private readonly Mock<IRentalRepository> _mockRentalRepository;

    public CreateRentalValidatorTests()
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
    public void ShouldHaveErrorWhenMotorcycleIdIsEmpty()
    {
        // Arrange
        var model = new CreateRentalDto
        {
            MotorcycleId = Guid.Empty,
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            PlanType = RentalPlanType.SevenDays
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MotorcycleId);
    }

    [Fact]
    public void ShouldHaveErrorWhenDeliveryPersonIdIsEmpty()
    {
        // Arrange
        var model = new CreateRentalDto
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.Empty,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            PlanType = RentalPlanType.SevenDays
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeliveryPersonId);
    }

    [Fact]
    public void ShouldHaveErrorWhenStartDateIsInPast()
    {
        // Arrange
        var model = new CreateRentalDto
        {
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            PlanType = RentalPlanType.SevenDays
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void ShouldPassValidationWithValidModel()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        
        var model = new CreateRentalDto
        {
            MotorcycleId = motorcycleId,
            DeliveryPersonId = deliveryPersonId,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            PlanType = RentalPlanType.SevenDays
        };

        // Configure mocks
        _mockMotorcycleRepository.Setup(repo => repo.GetByIdAsync(motorcycleId, default))
            .ReturnsAsync(new MotorcycleEntity());
            
        _mockRentalRepository.Setup(repo => repo.ExistsActiveRentalForMotorcycleAsync(motorcycleId, default))
            .ReturnsAsync(false);
            
        var deliveryPerson = new Mock<DeliveryPersonEntity>();
        deliveryPerson.Setup(dp => dp.CanRentMotorcycle()).Returns(true);
        
        _mockDeliveryPersonRepository.Setup(repo => repo.GetByIdAsync(deliveryPersonId, default))
            .ReturnsAsync(deliveryPerson.Object);
            
        _mockRentalRepository.Setup(repo => repo.GetActiveRentalByDeliveryPersonIdAsync(deliveryPersonId, default))
            .ReturnsAsync((RentalEntity)null);

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 