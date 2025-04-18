using FluentAssertions;
using FluentValidation.TestHelper;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Validators;
using Xunit;

namespace Motorcycle.Application.Tests.Validation;

public class UpdateMotorcycleValidatorTests
{
    private readonly UpdateMotorcycleDtoValidator _validator;

    public UpdateMotorcycleValidatorTests()
    {
        _validator = new UpdateMotorcycleDtoValidator();
    }

    [Fact]
    public void ShouldHaveErrorWhenModelIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = string.Empty,
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public void ShouldHaveErrorWhenBrandIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = string.Empty,
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brand);
    }

    [Fact]
    public void ShouldHaveErrorWhenColorIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = string.Empty,
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Fact]
    public void ShouldHaveErrorWhenManufacturingYearIsZero()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 0,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ManufacturingYear);
    }

    [Fact]
    public void ShouldHaveErrorWhenManufacturingYearIsInFuture()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = DateTime.UtcNow.Year + 1,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ManufacturingYear);
    }

    [Fact]
    public void ShouldHaveErrorWhenPlateNumberIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = string.Empty,
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PlateNumber);
    }

    [Fact]
    public void ShouldHaveErrorWhenPlateNumberFormatIsInvalid()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "INVALID",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PlateNumber);
    }

    [Fact]
    public void ShouldHaveErrorWhenDailyRateIsZero()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = 0
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DailyRate);
    }

    [Fact]
    public void ShouldHaveErrorWhenDailyRateIsNegative()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = -50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DailyRate);
    }

    [Fact]
    public void ShouldHaveErrorWhenIdIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.Empty,
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void ShouldPassValidationWithValidModel()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC-1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 