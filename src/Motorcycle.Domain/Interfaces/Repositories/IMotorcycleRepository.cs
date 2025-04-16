using Motorcycle.Domain.Entities;

namespace Motorcycle.Domain.Interfaces.Repositories;

public interface IMotorcycleRepository : IBaseRepository<MotorcycleEntity>
{
    Task<MotorcycleEntity?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<bool> ExistsByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<IEnumerable<MotorcycleEntity>> SearchByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
}