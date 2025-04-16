namespace Motorcycle.Infrastructure.Data.Entities;

public class Motorcycle2024Notification
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public DateTime NotificationTimestamp { get; set; }
    public DateTime CreatedAt { get; set; }
}