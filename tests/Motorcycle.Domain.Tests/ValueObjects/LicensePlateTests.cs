using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Motorcycle.Domain.Tests.ValueObjects;

public class LicensePlateTests
{
    [Theory]
    [InlineData("ABC1234")] // Padrão antigo
    [InlineData("ABC1D23")] // Padrão Mercosul
    [InlineData("CDX-0101")] // Padrão com hífen
    public void Create_WithValidLicensePlate_ShouldCreateLicensePlate(string validLicensePlate)
    {
        // Act
        var licensePlate = LicensePlate.Create(validLicensePlate);

        // Assert
        licensePlate.Should().NotBeNull();
        licensePlate.Value.Should().Be(validLicensePlate.Trim().ToUpper());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyLicensePlate_ShouldThrowDomainException(string emptyLicensePlate)
    {
        // Act & Assert
        var action = () => LicensePlate.Create(emptyLicensePlate);
        action.Should().Throw<DomainException>()
            .WithMessage("A placa não pode ser vazia.");
    }

    [Theory]
    [InlineData("AB12345")] // Formato inválido
    [InlineData("ABCD123")] // Formato inválido
    [InlineData("123ABCD")] // Formato inválido
    [InlineData("ABC-123")] // Formato inválido
    [InlineData("ABC123")] // Formato inválido
    public void Create_WithInvalidLicensePlate_ShouldThrowDomainException(string invalidLicensePlate)
    {
        // Act & Assert
        var action = () => LicensePlate.Create(invalidLicensePlate);
        action.Should().Throw<DomainException>()
            .WithMessage("Formato de placa inválido.");
    }

    [Fact]
    public void Equals_WithSameLicensePlate_ShouldReturnTrue()
    {
        // Arrange
        var licensePlate1 = LicensePlate.Create("ABC1234");
        var licensePlate2 = LicensePlate.Create("ABC1234");

        // Act & Assert
        licensePlate1.Equals(licensePlate2).Should().BeTrue();
        (licensePlate1 == licensePlate2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentLicensePlate_ShouldReturnFalse()
    {
        // Arrange
        var licensePlate1 = LicensePlate.Create("ABC1234");
        var licensePlate2 = LicensePlate.Create("XYZ5678");

        // Act & Assert
        licensePlate1.Equals(licensePlate2).Should().BeFalse();
        (licensePlate1 != licensePlate2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var licensePlate = LicensePlate.Create("ABC1234");

        // Act & Assert
        licensePlate.Equals(null).Should().BeFalse();
    }
    
    [Fact]
    public void GetHashCode_WithSameLicensePlate_ShouldReturnSameHashCode()
    {
        // Arrange
        var licensePlate1 = LicensePlate.Create("ABC1234");
        var licensePlate2 = LicensePlate.Create("ABC1234");

        // Act & Assert
        licensePlate1.GetHashCode().Should().Be(licensePlate2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var licensePlateValue = "ABC1234";
        var licensePlate = LicensePlate.Create(licensePlateValue);

        // Act
        var result = licensePlate.ToString();

        // Assert
        result.Should().Be(licensePlateValue);
    }

    [Fact]
    public void OperatorEqual_WithNullReferences_ShouldReturnTrue()
    {
        // Arrange
        LicensePlate? licensePlate1 = null;
        LicensePlate? licensePlate2 = null;

        // Act & Assert
        (licensePlate1 == licensePlate2).Should().BeTrue();
    }

    [Fact]
    public void OperatorEqual_WithOneNullReference_ShouldReturnFalse()
    {
        // Arrange
        var licensePlate1 = LicensePlate.Create("ABC1234");
        LicensePlate? licensePlate2 = null;

        // Act & Assert
        (licensePlate1 == licensePlate2).Should().BeFalse();
        (licensePlate2 == licensePlate1).Should().BeFalse();
    }

    [Fact]
    public void OperatorNotEqual_WithDifferentInstances_ShouldReturnTrue()
    {
        // Arrange
        var licensePlate1 = LicensePlate.Create("ABC1234");
        var licensePlate2 = LicensePlate.Create("XYZ9876");

        // Act & Assert
        (licensePlate1 != licensePlate2).Should().BeTrue();
    }
} 