namespace Motorcycle.Domain.Events;

public class MotorcycleCreatedEvent : DomainEvent
{
    public Guid MotorcycleId { get; }
    public string LicensePlate { get; }
    public int Year { get; }
    public string Model { get; }

    public MotorcycleCreatedEvent(Guid motorcycleId, string licensePlate, int year, string model)
        : base()
    {
        MotorcycleId = motorcycleId;
        LicensePlate = licensePlate;
        Year = year;
        Model = model;
    }
}