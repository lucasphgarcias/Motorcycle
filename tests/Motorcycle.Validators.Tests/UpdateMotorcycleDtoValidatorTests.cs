using FluentAssertions;
using FluentValidation.TestHelper;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Validators;
using Xunit;

namespace Motorcycle.Validators.Tests;

public class UpdateMotorcycleDtoValidatorTests
{
    private readonly UpdateMotorcycleDtoValidator _validator;

    public UpdateMotorcycleDtoValidatorTests()
    {
        _validator = new UpdateMotorcycleDtoValidator();
    }

    [Fact]
    public async Task ShouldHaveErrorWhenIdIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.Empty,
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC1234",
            DailyRate = 50.00m
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task ShouldHaveErrorWhenModelIsEmpty()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = string.Empty,
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC1234",
            DailyRate = 50.00m
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public async Task ShouldPassValidationWithValidModel()
    {
        // Arrange
        var dto = new UpdateMotorcycleDto
        {
            Id = Guid.NewGuid(),
            Model = "CB300",
            Brand = "Honda",
            Color = "Black",
            ManufacturingYear = 2020,
            PlateNumber = "ABC1234",
            DailyRate = 50.00m
        };

        // Act
        var result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 