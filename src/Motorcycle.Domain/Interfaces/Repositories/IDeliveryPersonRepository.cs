using Motorcycle.Domain.Entities;

namespace Motorcycle.Domain.Interfaces.Repositories;

public interface IDeliveryPersonRepository : IBaseRepository<DeliveryPersonEntity>
{
    Task<DeliveryPersonEntity?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<DeliveryPersonEntity?> GetByDriverLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDriverLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
}