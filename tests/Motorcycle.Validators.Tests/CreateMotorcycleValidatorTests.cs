using FluentAssertions;
using FluentValidation.TestHelper;
using Moq;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Validators;
using Motorcycle.Domain.Interfaces.Repositories;
using Xunit;

namespace Motorcycle.Validators.Tests;

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
    public async Task ShouldHaveErrorWhenModelIsEmpty()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = string.Empty,
            Year = 2022,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Fact]
    public async Task ShouldPassWithValidModel()
    {
        // Arrange
        var model = new CreateMotorcycleDto
        {
            Model = "CG 160",
            Year = 2022,
            LicensePlate = "ABC1234"
        };

        // Act
        var result = await _validator.TestValidateAsync(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 