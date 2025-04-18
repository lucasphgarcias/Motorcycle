using Motorcycle.Domain.Entities;

namespace Motorcycle.Domain.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(UserEntity user);
}