namespace Motorcycle.Application.Interfaces;

public interface IBaseService<TDto, TCreateDto, TId>
{
    Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TDto?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
    Task<TDto> UpdateAsync(TId id, TCreateDto updateDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}