using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Infrastructure.Data.Context;

namespace Motorcycle.Infrastructure.Data.Repositories;

public class MotorcycleRepository : BaseRepository<MotorcycleEntity>, IMotorcycleRepository
{
    public MotorcycleRepository(MotorcycleDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<MotorcycleEntity?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Motorcycles
            .FirstOrDefaultAsync(m => 
                EF.Property<string>(m, "LicensePlate") == licensePlate, 
                cancellationToken);
    }

    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Motorcycles
            .Where(m => m.LicensePlate.Value == licensePlate) // Acessando a propriedade Value diretamente
            .AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<MotorcycleEntity>> SearchByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Motorcycles
            .Where(m => m.LicensePlate.Value.Contains(licensePlate)) // Access the Value property
            .ToListAsync(cancellationToken);
    }

    public override async Task<MotorcycleEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Motorcycles
            .Include(m => m.Rentals)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<MotorcycleEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Motorcycles
            .Include(m => m.Rentals)
            .ToListAsync(cancellationToken);
    }
}