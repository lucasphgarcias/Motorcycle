using Motorcycle.Domain.Events;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;

namespace Motorcycle.Domain.Entities;

public class MotorcycleEntity : Entity
{
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public LicensePlate LicensePlate { get; private set; } = null!;
    public List<RentalEntity> Rentals { get; private set; } = new();
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // Para EF Core
    private MotorcycleEntity() : base() { }

    private MotorcycleEntity(
        string model,
        int year,
        LicensePlate licensePlate)
        : base()
    {
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
    }

    public static MotorcycleEntity Create(string model, int year, string licensePlate)
    {
        ValidateModel(model);
        ValidateYear(year);
        var licensePlateVO = LicensePlate.Create(licensePlate);

        var motorcycle = new MotorcycleEntity(model, year, licensePlateVO);
        
        // Adiciona o evento de criação da moto
        motorcycle.AddDomainEvent(new MotorcycleCreatedEvent(
            motorcycle.Id,
            licensePlateVO.Value,
            year,
            model
        ));

        return motorcycle;
    }

    public void UpdateLicensePlate(string licensePlate)
    {
        var newLicensePlate = LicensePlate.Create(licensePlate);
        LicensePlate = newLicensePlate;
    }

    public bool CanBeRemoved()
    {
        return !Rentals.Any();
    }

    private static void ValidateModel(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("O modelo da moto não pode ser vazio.");
    }

    private static void ValidateYear(int year)
    {
        var currentYear = DateTime.Now.Year;
        if (year < 1900 || year > currentYear + 1) // Permite o ano atual e o próximo
            throw new DomainException($"O ano deve estar entre 1900 e {currentYear + 1}.");
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}