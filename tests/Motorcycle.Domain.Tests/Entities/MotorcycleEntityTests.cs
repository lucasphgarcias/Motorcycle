using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Events;
using FluentAssertions;
using Xunit;

namespace Motorcycle.Domain.Tests.Entities;

public class MotorcycleEntityTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateMotorcycle()
    {
        // Arrange
        var model = "Honda CG 160";
        var year = 2023;
        var licensePlate = "ABC1234";

        // Act
        var motorcycle = MotorcycleEntity.Create(model, year, licensePlate);

        // Assert
        motorcycle.Should().NotBeNull();
        motorcycle.Model.Should().Be(model);
        motorcycle.Year.Should().Be(year);
        motorcycle.LicensePlate.Value.Should().Be(licensePlate);
        motorcycle.DomainEvents.Should().ContainSingle(e => e is MotorcycleCreatedEvent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidModel_ShouldThrowDomainException(string invalidModel)
    {
        // Arrange
        var year = 2023;
        var licensePlate = "ABC1234";

        // Act & Assert
        var action = () => MotorcycleEntity.Create(invalidModel, year, licensePlate);
        action.Should().Throw<DomainException>()
            .WithMessage("O modelo da moto não pode ser vazio.");
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(3000)]
    public void Create_WithInvalidYear_ShouldThrowDomainException(int invalidYear)
    {
        // Arrange
        var model = "Honda CG 160";
        var licensePlate = "ABC1234";
        var currentYear = DateTime.Now.Year;

        // Act & Assert
        var action = () => MotorcycleEntity.Create(model, invalidYear, licensePlate);
        action.Should().Throw<DomainException>()
            .WithMessage($"O ano deve estar entre 1900 e {currentYear + 1}.");
    }

    [Fact]
    public void UpdateLicensePlate_WithValidLicensePlate_ShouldUpdateLicensePlate()
    {
        // Arrange
        var motorcycle = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
        var newLicensePlate = "XYZ5678";

        // Act
        motorcycle.UpdateLicensePlate(newLicensePlate);

        // Assert
        motorcycle.LicensePlate.Value.Should().Be(newLicensePlate);
    }

    [Fact]
    public void CanBeRemoved_WithNoRentals_ShouldReturnTrue()
    {
        // Arrange
        var motorcycle = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");

        // Act
        var result = motorcycle.CanBeRemoved();

        // Assert
        result.Should().BeTrue();
    }
} 