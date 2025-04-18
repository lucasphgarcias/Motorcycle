using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Motorcycle.Domain.Tests.ValueObjects;

public class CnpjTests
{
    [Theory]
    [InlineData("12345678000199")]
    [InlineData("98765432000188")]
    [InlineData("34567890000177")]
    public void Create_WithValidCnpj_ShouldCreateCnpj(string value)
    {
        // Act
        var cnpj = Cnpj.Create(value);

        // Assert
        cnpj.Should().NotBeNull();
        // Verifica se o valor está formatado
        cnpj.Value.Should().Contain(".");
        cnpj.Value.Should().Contain("/");
        cnpj.Value.Should().Contain("-");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyCnpj_ShouldThrowDomainException(string invalidValue)
    {
        // Act & Assert
        var action = () => Cnpj.Create(invalidValue);
        action.Should().Throw<DomainException>()
            .WithMessage("O CNPJ não pode ser vazio.");
    }

    [Theory]
    [InlineData("1234567800")]
    [InlineData("123456780001999")]
    [InlineData("1234567800019")]
    [InlineData("123456780001")]
    [InlineData("abcdefghijklmn")]
    [InlineData("1234567800019a")]
    public void Create_WithInvalidCnpjFormat_ShouldThrowDomainException(string invalidValue)
    {
        // Act & Assert
        var action = () => Cnpj.Create(invalidValue);
        action.Should().Throw<DomainException>()
            .WithMessage("CNPJ inválido.");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedCnpj()
    {
        // Arrange
        var cnpj = Cnpj.Create("12345678000199");

        // Act
        var result = cnpj.ToString();

        // Assert
        result.Should().Contain(".");
        result.Should().Contain("/");
        result.Should().Contain("-");
    }

    [Fact]
    public void Equals_WithSameCnpj_ShouldReturnTrue()
    {
        // Arrange
        var cnpj1 = Cnpj.Create("12345678000199");
        var cnpj2 = Cnpj.Create("12345678000199");

        // Act & Assert
        cnpj1.Equals(cnpj2).Should().BeTrue();
        (cnpj1 == cnpj2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCnpj_ShouldReturnFalse()
    {
        // Arrange
        var cnpj1 = Cnpj.Create("12345678000199");
        var cnpj2 = Cnpj.Create("98765432000188");

        // Act & Assert
        cnpj1.Equals(cnpj2).Should().BeFalse();
        (cnpj1 != cnpj2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var cnpj = Cnpj.Create("12345678000199");

        // Act & Assert
        cnpj.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameCnpj_ShouldReturnSameHashCode()
    {
        // Arrange
        var cnpj1 = Cnpj.Create("12345678000199");
        var cnpj2 = Cnpj.Create("12345678000199");

        // Act & Assert
        cnpj1.GetHashCode().Should().Be(cnpj2.GetHashCode());
    }
} 