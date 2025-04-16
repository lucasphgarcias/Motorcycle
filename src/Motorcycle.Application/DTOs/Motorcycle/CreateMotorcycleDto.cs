namespace Motorcycle.Application.DTOs.Motorcycle;

public class CreateMotorcycleDto
{
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
}