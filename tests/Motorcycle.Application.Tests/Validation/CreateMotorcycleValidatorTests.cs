using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Validators;
using Motorcycle.Domain.Interfaces.Repositories;
using Xunit;

namespace Motorcycle.Application.Tests.Validation;

public class CreateMotorcycleValidatorTests
{
    private readonly CreateMotorcycleValidator _validator;
    private readonly Mock<IMotorcycleRepository> _mockRepository;

    public CreateMotorcycleValidatorTests()
    {
        _mockRepository = new Mock<IMotorcycleRepository>();
        _mockRepository.Setup(r => r.ExistsByLicensePlateAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);
            
        _validator = new CreateMotorcycleValidator(_mockRepository.Object);
    }

    [Fact]
    public void ShouldHaveErrorWhenModelIsEmpty()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "",
            Year = 2022,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public void ShouldHaveErrorWhenYearIsZero()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "CG 160",
            Year = 0,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void ShouldHaveErrorWhenYearIsInFuture()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "CG 160",
            Year = DateTime.UtcNow.Year + 2,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void ShouldHaveErrorWhenLicensePlateIsEmpty()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "CG 160",
            Year = 2022,
            LicensePlate = ""
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Fact]
    public void ShouldHaveErrorWhenLicensePlateFormatIsInvalid()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "CG 160",
            Year = 2022,
            LicensePlate = "invalid"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LicensePlate);
    }

    [Fact]
    public void ShouldPassValidationWithValidModel()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "CG 160",
            Year = 2022,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 