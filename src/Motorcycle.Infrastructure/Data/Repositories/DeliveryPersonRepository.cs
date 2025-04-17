using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Infrastructure.Data.Context;

namespace Motorcycle.Infrastructure.Data.Repositories;

public class DeliveryPersonRepository : BaseRepository<DeliveryPersonEntity>, IDeliveryPersonRepository
{
    public DeliveryPersonRepository(MotorcycleDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<DeliveryPersonEntity?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        // return await _dbContext.DeliveryPersons
        //     .FirstOrDefaultAsync(d => 
        //         EF.Property<string>(d, "Cnpj") == cnpj, 
        //         cancellationToken);

        return await _dbContext.DeliveryPersons
            .FirstOrDefaultAsync(d => d.Cnpj.Value == cnpj, cancellationToken);
    }

    public async Task<DeliveryPersonEntity?> GetByDriverLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        // return await _dbContext.DeliveryPersons
        //     .FirstOrDefaultAsync(d => 
        //         EF.Property<string>(d, "LicenseNumber") == licenseNumber, 
        //         cancellationToken);

        return await _dbContext.DeliveryPersons
        .FirstOrDefaultAsync(d => d.DriverLicense.Number == licenseNumber, cancellationToken);
    }

    public async Task<bool> ExistsByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        // return await _dbContext.DeliveryPersons
        //     .AnyAsync(d => 
        //         EF.Property<string>(d, "Cnpj") == cnpj, 
        //         cancellationToken);

        return await _dbContext.DeliveryPersons
            .AnyAsync(d => d.Cnpj.Value == cnpj, cancellationToken);
    }

    public async Task<bool> ExistsByDriverLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        // return await _dbContext.DeliveryPersons
        //     .AnyAsync(d => 
        //         EF.Property<string>(d, "LicenseNumber") == licenseNumber, 
        //         cancellationToken);

        return await _dbContext.DeliveryPersons
        .AnyAsync(d => d.DriverLicense.Number == licenseNumber, cancellationToken);
    }

    public override async Task<DeliveryPersonEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DeliveryPersons
            .Include(d => d.Rentals)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<DeliveryPersonEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DeliveryPersons
            .Include(d => d.Rentals)
            .ToListAsync(cancellationToken);
    }
}