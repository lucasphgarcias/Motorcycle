using Motorcycle.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Motorcycle.Domain.ValueObjects;

public class LicensePlate : IEquatable<LicensePlate>
{
    public string Value { get; }

    private LicensePlate(string value)
    {
        Value = value;
    }

    public static LicensePlate Create(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new DomainException("A placa não pode ser vazia.");

        // Formata a placa removendo espaços e transformando em maiúsculo
        var formattedLicensePlate = licensePlate.Trim().ToUpper();

        // Valida o formato da placa (padrão brasileiro)
        if (!IsValidLicensePlate(formattedLicensePlate))
            throw new DomainException("Formato de placa inválido.");

        return new LicensePlate(formattedLicensePlate);
    }

    private static bool IsValidLicensePlate(string licensePlate)
    {
        // Padrão antigo: ABC1234
        var oldFormatPattern = @"^[A-Z]{3}\d{4}$";
        
        // Padrão Mercosul: ABC1D23
        var mercosulFormatPattern = @"^[A-Z]{3}\d[A-Z]\d{2}$";
        
        // Padrão com hífen: CDX-0101
        var hyphenFormatPattern = @"^[A-Z]{3}-\d{4}$";
        
        return Regex.IsMatch(licensePlate, oldFormatPattern) || 
            Regex.IsMatch(licensePlate, mercosulFormatPattern) ||
            Regex.IsMatch(licensePlate, hyphenFormatPattern);
    }

    public bool Equals(LicensePlate? other)
    {
        if (other is null)
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is LicensePlate other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(LicensePlate? left, LicensePlate? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }

    public static bool operator !=(LicensePlate? left, LicensePlate? right)
    {
        return !(left == right);
    }
}