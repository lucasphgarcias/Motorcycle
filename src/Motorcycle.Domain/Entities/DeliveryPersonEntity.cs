using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;

namespace Motorcycle.Domain.Entities;

public class DeliveryPersonEntity : Entity
{
    public required string Name { get; set; }
    public required Cnpj Cnpj { get; set; }
    public DateOnly BirthDate { get; private set; }
    public required DriverLicense DriverLicense { get; set; }
    public List<RentalEntity> Rentals { get; private set; } = new();

    // Para EF Core
    private DeliveryPersonEntity() : base() { }

    private DeliveryPersonEntity(
        string name,
        Cnpj cnpj,
        DateOnly birthDate,
        DriverLicense driverLicense)
        : base()
    {
        Name = name;
        Cnpj = cnpj;
        BirthDate = birthDate;
        DriverLicense = driverLicense;
    }

    public static DeliveryPersonEntity Create(
        string name,
        string cnpj,
        DateOnly birthDate,
        string licenseNumber,
        Enums.LicenseType licenseType)
    {
        ValidateName(name);
        ValidateBirthDate(birthDate);

        var cnpjVO = Cnpj.Create(cnpj);
        var driverLicense = DriverLicense.Create(licenseNumber, licenseType);

        return new DeliveryPersonEntity(name, cnpjVO, birthDate, driverLicense)
        {
            Name = name,
            Cnpj = cnpjVO,
            DriverLicense = driverLicense
        };
    }

    public void UpdateDriverLicenseImage(string imagePath)
    {
        DriverLicense.UpdateImagePath(imagePath);
    }

    public bool CanRentMotorcycle()
    {
        return DriverLicense.CanDriveMotorcycle();
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do entregador não pode ser vazio.");
    }

    private static void ValidateBirthDate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var minimumAge = 18;
        var minimumBirthDate = today.AddYears(-minimumAge);

        if (birthDate > minimumBirthDate)
            throw new DomainException($"O entregador deve ter no mínimo {minimumAge} anos.");
    }
}