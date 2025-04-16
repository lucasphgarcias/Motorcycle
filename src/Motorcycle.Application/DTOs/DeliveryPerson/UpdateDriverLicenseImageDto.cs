using Microsoft.AspNetCore.Http;

namespace Motorcycle.Application.DTOs.DeliveryPerson;

public class UpdateDriverLicenseImageDto
{
    public IFormFile LicenseImage { get; set; } = null!;
}
