namespace Motorcycle.Domain.Events;

public class Motorcycle2024CreatedEvent : DomainEvent
{
    public Guid MotorcycleId { get; }
    public string LicensePlate { get; }
    public string Model { get; }
    public DateTime NotificationTimestamp { get; }

    public Motorcycle2024CreatedEvent(Guid motorcycleId, string licensePlate, string model)
        : base()
    {
        MotorcycleId = motorcycleId;
        LicensePlate = licensePlate;
        Model = model;
        NotificationTimestamp = DateTime.UtcNow;
    }
}