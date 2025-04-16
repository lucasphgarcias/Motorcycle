using Motorcycle.Domain.Exceptions;

namespace Motorcycle.Domain.ValueObjects;

public class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; } = "BRL"; // Moeda brasileira como padrão

    private Money(decimal amount, string currency = "BRL")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "BRL")
    {
        if (amount < 0)
            throw new DomainException("O valor monetário não pode ser negativo.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("A moeda deve ser especificada.");

        // Arredonda para duas casas decimais
        var roundedAmount = Math.Round(amount, 2);

        return new Money(roundedAmount, currency);
    }

    public static Money Zero() => new Money(0);

    public Money Add(Money money)
    {
        if (money.Currency != Currency)
            throw new DomainException("Não é possível somar valores de moedas diferentes.");

        return new Money(Amount + money.Amount, Currency);
    }

    public Money Subtract(Money money)
    {
        if (money.Currency != Currency)
            throw new DomainException("Não é possível subtrair valores de moedas diferentes.");

        var result = Amount - money.Amount;
        if (result < 0)
            throw new DomainException("O resultado da subtração não pode ser um valor negativo.");

        return new Money(result, Currency);
    }

    public Money Multiply(decimal multiplier)
    {
        if (multiplier < 0)
            throw new DomainException("O multiplicador não pode ser negativo.");

        return new Money(Amount * multiplier, Currency);
    }

    public bool Equals(Money? other)
    {
        if (other is null)
            return false;

        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public override string ToString()
    {
        return $"{Currency} {Amount:F2}";
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }

    public static Money operator +(Money left, Money right)
    {
        return left.Add(right);
    }

    public static Money operator -(Money left, Money right)
    {
        return left.Subtract(right);
    }

    public static Money operator *(Money left, decimal multiplier)
    {
        return left.Multiply(multiplier);
    }
}