using Motorcycle.Application.DTOs.Motorcycle;

namespace Motorcycle.Application.Interfaces;

public interface IMotorcycleService : IBaseService<MotorcycleDto, CreateMotorcycleDto, Guid>
{
    Task<IEnumerable<MotorcycleDto>> SearchByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken = default);
    Task<MotorcycleDto> UpdateLicensePlateAsync(Guid id, UpdateMotorcycleLicensePlateDto updateDto, CancellationToken cancellationToken = default);
}