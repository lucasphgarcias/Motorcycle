using Motorcycle.Domain.Enums;
using System.ComponentModel;

namespace Motorcycle.Application.DTOs.Auth;

public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    
    /// <summary>
    /// Função do usuário: 0(User) ou 1(Admin)
    /// </summary>
    [Description("0(User) ou 1(Admin)")]
    public UserRole Role { get; set; } = UserRole.User;
}