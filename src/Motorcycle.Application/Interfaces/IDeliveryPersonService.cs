using Microsoft.AspNetCore.Http;
using Motorcycle.Application.DTOs.DeliveryPerson;


namespace Motorcycle.Application.Interfaces;

public interface IDeliveryPersonService : IBaseService<DeliveryPersonDto, CreateDeliveryPersonDto, Guid>
{
    Task<DeliveryPersonDto> UpdateDriverLicenseImageAsync(Guid id, IFormFile image, CancellationToken cancellationToken = default);
    Task<DeliveryPersonDto?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<DeliveryPersonDto?> GetByDriverLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
}