namespace Motorcycle.Application.DTOs.Motorcycle;

public class UpdateMotorcycleDto
{
    public Guid Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int ManufacturingYear { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
} 