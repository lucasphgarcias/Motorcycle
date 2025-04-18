using FluentValidation.TestHelper;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Validators;
using System;
using Xunit;

namespace Motorcycle.Application.Tests.Validators;

public class CreateMotorcycleValidatorTests
{
    private readonly CreateMotorcycleValidator _validator;

    public CreateMotorcycleValidatorTests()
    {
        _validator = new CreateMotorcycleValidator();
    }

    [Fact]
    public void Validator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = "CG 160",
            Year = DateTime.Now.Year,
            LicensePlate = "ABC1D23",
            Color = "Red",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("A")]  // Too short
    public void Brand_WithInvalidValue_ShouldHaveError(string brand)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = brand,
            Model = "CG 160",
            Year = DateTime.Now.Year,
            LicensePlate = "ABC1D23",
            Color = "Red",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Brand);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("A")]  // Too short
    public void Model_WithInvalidValue_ShouldHaveError(string model)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = model,
            Year = DateTime.Now.Year,
            LicensePlate = "ABC1D23",
            Color = "Red",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Theory]
    [InlineData(1990)]  // Too old
    [InlineData(2050)]  // Future year
    public void Year_WithInvalidValue_ShouldHaveError(int year)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = "CG 160",
            Year = year,
            LicensePlate = "ABC1D23",
            Color = "Red",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("INVALID")]
    [InlineData("AB12345")]
    [InlineData("A1B2C3")]
    public void LicensePlate_WithInvalidFormat_ShouldHaveError(string licensePlate)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = "CG 160",
            Year = DateTime.Now.Year,
            LicensePlate = licensePlate,
            Color = "Red",
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Color_WithInvalidValue_ShouldHaveError(string color)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = "CG 160",
            Year = DateTime.Now.Year,
            LicensePlate = "ABC1D23",
            Color = color,
            DailyRate = 50.00m
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Color);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50.5)]
    public void DailyRate_WithInvalidValue_ShouldHaveError(decimal dailyRate)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Brand = "Honda",
            Model = "CG 160",
            Year = DateTime.Now.Year,
            LicensePlate = "ABC1D23",
            Color = "Red",
            DailyRate = dailyRate
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DailyRate);
    }
} 