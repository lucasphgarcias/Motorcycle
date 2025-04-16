using Motorcycle.Domain.Entities;

namespace Motorcycle.Domain.Interfaces.Repositories;

public interface IRentalRepository : IBaseRepository<RentalEntity>
{
    Task<IEnumerable<RentalEntity>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RentalEntity>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveRentalForMotorcycleAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
    Task<RentalEntity?> GetActiveRentalByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
}