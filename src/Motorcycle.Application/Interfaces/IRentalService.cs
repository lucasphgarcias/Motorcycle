using Motorcycle.Application.DTOs.Rental;

namespace Motorcycle.Application.Interfaces;

public interface IRentalService : IBaseService<RentalDto, CreateRentalDto, Guid>
{
    Task<IEnumerable<RentalDto>> GetByMotorcycleIdAsync(Guid motorcycleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RentalDto>> GetByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<RentalDto?> GetActiveRentalByDeliveryPersonIdAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<RentalTotalAmountDto> ReturnMotorcycleAsync(Guid id, ReturnMotorcycleDto returnDto, CancellationToken cancellationToken = default);
    Task<RentalTotalAmountDto> CalculateTotalAmountAsync(Guid id, DateOnly returnDate, CancellationToken cancellationToken = default);
}