using Motorcycle.Domain.Enums;

namespace Motorcycle.Application.DTOs.DeliveryPerson{

    public class CreateDeliveryPersonDto
    {
        public string Name { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public LicenseType LicenseType { get; set; }
    }

}

