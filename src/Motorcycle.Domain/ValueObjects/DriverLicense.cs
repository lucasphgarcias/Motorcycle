using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace Motorcycle.Domain.ValueObjects;

public class DriverLicense : IEquatable<DriverLicense>
{
    public string Number { get; }
    public LicenseType Type { get; }
    public string ImagePath { get; private set; }

    private DriverLicense(string number, LicenseType type, string imagePath = "")
    {
        Number = number;
        Type = type;
        ImagePath = imagePath;
    }

    public static DriverLicense Create(string number, LicenseType type, string imagePath = "")
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new DomainException("O número da CNH não pode ser vazio.");

        var formattedNumber = number.Trim();

        if (!IsValidLicenseNumber(formattedNumber))
            throw new DomainException("Número de CNH inválido.");

        return new DriverLicense(formattedNumber, type, imagePath);
    }

    public void UpdateImagePath(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new DomainException("O caminho da imagem não pode ser vazio.");

        ImagePath = imagePath;
    }

    private static bool IsValidLicenseNumber(string number)
    {
        // CNH brasileira tem 11 dígitos numéricos
        return Regex.IsMatch(number, @"^\d{11}$");
    }

    public bool CanDriveMotorcycle()
    {
        return Type == LicenseType.A || Type == LicenseType.AB;
    }

    public bool Equals(DriverLicense? other)
    {
        if (other is null)
            return false;

        return Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        return obj is DriverLicense other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Number.GetHashCode();
    }

    public static bool operator ==(DriverLicense? left, DriverLicense? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }

    public static bool operator !=(DriverLicense? left, DriverLicense? right)
    {
        return !(left == right);
    }
}