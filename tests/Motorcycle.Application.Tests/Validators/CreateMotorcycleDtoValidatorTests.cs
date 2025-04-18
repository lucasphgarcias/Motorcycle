using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Validators;
using Motorcycle.Domain.Interfaces.Repositories;
using System;
using Xunit;

namespace Motorcycle.Application.Tests.Validators;

public class CreateMotorcycleDtoValidatorTests
{
    private readonly CreateMotorcycleValidator _validator;
    private readonly Mock<IMotorcycleRepository> _mockRepository;

    public CreateMotorcycleDtoValidatorTests()
    {
        _mockRepository = new Mock<IMotorcycleRepository>();
        _mockRepository.Setup(r => r.ExistsByLicensePlateAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);
        
        _validator = new CreateMotorcycleValidator(_mockRepository.Object);
    }

    [Fact]
    public void Validator_WithValidData_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Model = "XYZ 125",
            Year = 2021,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void Model_WithInvalidValue_ShouldHaveError(string model)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Model = model,
            Year = 2021,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(1899)]
    [InlineData(2100)]
    public void Year_WithInvalidValue_ShouldHaveError(int year)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Model = "XYZ 125",
            Year = year,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    [InlineData("A1")]
    [InlineData("ABCDEFG")]
    [InlineData("ABC12345")]
    public void LicensePlate_WithInvalidValue_ShouldHaveError(string plateNumber)
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Model = "XYZ 125",
            Year = 2021,
            LicensePlate = plateNumber
        };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }
} 