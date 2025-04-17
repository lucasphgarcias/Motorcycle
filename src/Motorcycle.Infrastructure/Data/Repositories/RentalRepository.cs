using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Infrastructure.Data.Context;

namespace Motorcycle.Infrastructure.Data.Repositories;

public class RentalRepository : BaseRepository<RentalEntity>, IRentalRepository
{
    public RentalRepository(MotorcycleDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<RentalEntity>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rentals
            .Include(r => r.Motorcycle)
            .Include(r => r.DeliveryPerson)
            .Where(r => r.MotorcycleId == motorcycleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RentalEntity>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rentals
            .Include(r => r.Motorcycle)
            .Include(r => r.DeliveryPerson)
            .Where(r => r.DeliveryPersonId == deliveryPersonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsActiveRentalForMotorcycleAsync(Guid motorcycleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rentals
            .Where(r => r.MotorcycleId == motorcycleId)
            .Where(r => r.TotalAmount == null)  // Assuming TotalAmount is a property on RentalEntity
            .AnyAsync(cancellationToken);
    }

    public async Task<RentalEntity?> GetActiveRentalByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rentals
            .Include(r => r.Motorcycle)
            .Include(r => r.DeliveryPerson)
            .Where(r => r.DeliveryPersonId == deliveryPersonId)
            .Where(r => r.TotalAmount == null)  // Assuming TotalAmount is a property on RentalEntity
            .FirstOrDefaultAsync(cancellationToken);
    }
    public override async Task<RentalEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rentals
            .Include(r => r.Motorcycle)
            .Include(r => r.DeliveryPerson)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<RentalEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Rentals
            .Include(r => r.Motorcycle)
            .Include(r => r.DeliveryPerson)
            .ToListAsync(cancellationToken);
    }
}