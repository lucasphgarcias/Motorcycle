using Motorcycle.Domain.Enums;

namespace Motorcycle.Application.DTOs.DeliveryPerson;

public class DeliveryPersonDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public DateTime  BirthDate { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public LicenseType LicenseType { get; set; }
    public string? LicenseImagePath { get; set; }
}