using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;
using Motorcycle.Domain.Enums;
using FluentAssertions;
using Xunit;
using System.Text.RegularExpressions;

namespace Motorcycle.Domain.Tests.ValueObjects;

public class DriverLicenseTests
{
    [Theory]
    [InlineData("12345678901", LicenseType.A)]
    [InlineData("98765432101", LicenseType.B)]
    [InlineData("11122233344", LicenseType.AB)]
    public void Create_WithValidParameters_ShouldCreateDriverLicense(string number, LicenseType type)
    {
        // Act
        var driverLicense = DriverLicense.Create(number, type);

        // Assert
        driverLicense.Should().NotBeNull();
        driverLicense.Number.Should().Be(number);
        driverLicense.Type.Should().Be(type);
        driverLicense.ImagePath.Should().BeEmpty();
    }

    [Theory]
    [InlineData("12345678901", LicenseType.A, "path/to/image.jpg")]
    [InlineData("98765432101", LicenseType.B, "license.png")]
    public void Create_WithImagePath_ShouldCreateDriverLicenseWithImagePath(string number, LicenseType type, string imagePath)
    {
        // Act
        var driverLicense = DriverLicense.Create(number, type, imagePath);

        // Assert
        driverLicense.Should().NotBeNull();
        driverLicense.Number.Should().Be(number);
        driverLicense.Type.Should().Be(type);
        driverLicense.ImagePath.Should().Be(imagePath);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyLicenseNumber_ShouldThrowDomainException(string invalidNumber)
    {
        // Act & Assert
        var action = () => DriverLicense.Create(invalidNumber, LicenseType.A);
        action.Should().Throw<DomainException>()
            .WithMessage("O número da CNH não pode ser vazio.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("abcdefghijk")]
    [InlineData("1234567890a")]
    public void Create_WithInvalidLicenseNumber_ShouldThrowDomainException(string invalidNumber)
    {
        // Act & Assert
        var action = () => DriverLicense.Create(invalidNumber, LicenseType.A);
        action.Should().Throw<DomainException>()
            .WithMessage("Número de CNH inválido.");
    }

    [Fact]
    public void UpdateImagePath_WithValidPath_ShouldUpdateImagePath()
    {
        // Arrange
        var driverLicense = DriverLicense.Create("12345678901", LicenseType.A);
        var newImagePath = "new/path/to/image.jpg";

        // Act
        driverLicense.UpdateImagePath(newImagePath);

        // Assert
        driverLicense.ImagePath.Should().Be(newImagePath);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateImagePath_WithEmptyPath_ShouldThrowDomainException(string invalidPath)
    {
        // Arrange
        var driverLicense = DriverLicense.Create("12345678901", LicenseType.A);

        // Act & Assert
        var action = () => driverLicense.UpdateImagePath(invalidPath);
        action.Should().Throw<DomainException>()
            .WithMessage("O caminho da imagem não pode ser vazio.");
    }

    [Theory]
    [InlineData(LicenseType.A, true)]
    [InlineData(LicenseType.AB, true)]
    [InlineData(LicenseType.B, false)]
    public void CanDriveMotorcycle_ShouldReturnCorrectValue(LicenseType licenseType, bool expectedResult)
    {
        // Arrange
        var driverLicense = DriverLicense.Create("12345678901", licenseType);

        // Act
        var result = driverLicense.CanDriveMotorcycle();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void Equals_WithSameNumber_ShouldReturnTrue()
    {
        // Arrange
        var license1 = DriverLicense.Create("12345678901", LicenseType.A);
        var license2 = DriverLicense.Create("12345678901", LicenseType.B); // Different type, same number

        // Act & Assert
        license1.Equals(license2).Should().BeTrue();
        (license1 == license2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentNumber_ShouldReturnFalse()
    {
        // Arrange
        var license1 = DriverLicense.Create("12345678901", LicenseType.A);
        var license2 = DriverLicense.Create("98765432101", LicenseType.A);

        // Act & Assert
        license1.Equals(license2).Should().BeFalse();
        (license1 != license2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var license = DriverLicense.Create("12345678901", LicenseType.A);

        // Act & Assert
        license.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameNumber_ShouldReturnSameHashCode()
    {
        // Arrange
        var license1 = DriverLicense.Create("12345678901", LicenseType.A);
        var license2 = DriverLicense.Create("12345678901", LicenseType.B); // Different type, same number

        // Act & Assert
        license1.GetHashCode().Should().Be(license2.GetHashCode());
    }
} 