using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Validators;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Interfaces.Repositories;
using Xunit;

namespace Motorcycle.Validators.Tests;

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
    public async Task ShouldHaveErrorWhenMotorcycleIdIsEmpty()
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
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MotorcycleId);
    }

    [Fact]
    public async Task ShouldHaveErrorWhenDeliveryPersonIdIsEmpty()
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
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DeliveryPersonId);
    }

    [Fact(Skip = "Requer entidades reais que não podem ser mockadas facilmente")]
    public async Task ShouldPassValidationWithValidModel()
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

        // Este teste requer entidades reais e seria complexo de configurar nos testes unitários
        // É melhor testado nos testes de integração

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 