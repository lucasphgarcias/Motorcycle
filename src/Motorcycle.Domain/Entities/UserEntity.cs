using Motorcycle.Domain.Enums;

namespace Motorcycle.Domain.Entities;

public class UserEntity
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLogin { get; private set; }

    private UserEntity(
        string username,
        string email,
        string passwordHash,
        UserRole role)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    public static UserEntity Create(
        string username,
        string email,
        string passwordHash,
        UserRole role = UserRole.User)
    {
        return new UserEntity(username, email, passwordHash, role);
    }

    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
    }
}