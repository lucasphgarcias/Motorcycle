using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using System.Globalization;

namespace Motorcycle.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(100, "BRL")]
    [InlineData(0, "BRL")]
    [InlineData(999.99, "USD")]
    public void Create_WithValidParameters_ShouldCreateMoney(decimal amount, string currency)
    {
        // Act
        var money = Money.Create(amount, currency);

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Arrange
        decimal negativeAmount = -10.50m;

        // Act & Assert
        var action = () => Money.Create(negativeAmount);
        action.Should().Throw<DomainException>()
            .WithMessage("O valor monetário não pode ser negativo.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidCurrency_ShouldThrowDomainException(string invalidCurrency)
    {
        // Arrange
        decimal amount = 100m;

        // Act & Assert
        var action = () => Money.Create(amount, invalidCurrency);
        action.Should().Throw<DomainException>()
            .WithMessage("A moeda deve ser especificada.");
    }

    [Fact]
    public void Zero_ShouldReturnMoneyWithZeroAmount()
    {
        // Act
        var money = Money.Zero();

        // Assert
        money.Amount.Should().Be(0);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(50, "BRL");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(50, "USD");

        // Act & Assert
        var action = () => money1.Add(money2);
        action.Should().Throw<DomainException>()
            .WithMessage("Não é possível somar valores de moedas diferentes.");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(30, "BRL");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Subtract_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(30, "USD");

        // Act & Assert
        var action = () => money1.Subtract(money2);
        action.Should().Throw<DomainException>()
            .WithMessage("Não é possível subtrair valores de moedas diferentes.");
    }

    [Fact]
    public void Subtract_ResultingInNegativeAmount_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(50, "BRL");
        var money2 = Money.Create(100, "BRL");

        // Act & Assert
        var action = () => money1.Subtract(money2);
        action.Should().Throw<DomainException>()
            .WithMessage("O resultado da subtração não pode ser um valor negativo.");
    }

    [Fact]
    public void Multiply_WithPositiveMultiplier_ShouldMultiplyAmount()
    {
        // Arrange
        var money = Money.Create(50, "BRL");
        decimal multiplier = 2.5m;

        // Act
        var result = money.Multiply(multiplier);

        // Assert
        result.Amount.Should().Be(125);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Multiply_WithNegativeMultiplier_ShouldThrowDomainException()
    {
        // Arrange
        var money = Money.Create(50, "BRL");
        decimal negativeMultiplier = -2;

        // Act & Assert
        var action = () => money.Multiply(negativeMultiplier);
        action.Should().Throw<DomainException>()
            .WithMessage("O multiplicador não pode ser negativo.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(100, "BRL");

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(200, "BRL");

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCurrencies_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(100, "USD");

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = Money.Create(123.45m, "BRL");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Match(s => s == "BRL 123.45" || s == "BRL 123,45");
    }

    [Fact]
    public void OperatorPlus_ShouldAddMoneyObjects()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(50, "BRL");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void OperatorMinus_ShouldSubtractMoneyObjects()
    {
        // Arrange
        var money1 = Money.Create(100, "BRL");
        var money2 = Money.Create(30, "BRL");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void OperatorMultiply_ShouldMultiplyMoneyByDecimal()
    {
        // Arrange
        var money = Money.Create(50, "BRL");
        decimal multiplier = 2;

        // Act
        var result = money * multiplier;

        // Assert
        result.Amount.Should().Be(100);
        result.Currency.Should().Be("BRL");
    }
} 