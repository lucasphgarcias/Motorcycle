using Motorcycle.Application.DTOs.Auth;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Motorcycle.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AuthService> _logger;
    
    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }
    
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Attempting login for user: {Username}", loginDto.Username);
        
        var user = await _userRepository.GetByUsernameAsync(loginDto.Username, cancellationToken);
        
        if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
        {
            throw new DomainException("Nome de usuário ou senha inválidos.");
        }
        
        // Update last login time
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);
        
        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        
        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }
    
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering new user: {Username}", registerDto.Username);
        
        // Validate password match
        if (registerDto.Password != registerDto.ConfirmPassword)
        {
            throw new DomainException("As senhas não coincidem.");
        }
        
        // Check if username exists
        if (await _userRepository.GetByUsernameAsync(registerDto.Username, cancellationToken) != null)
        {
            throw new DomainException($"Nome de usuário '{registerDto.Username}' já está em uso.");
        }
        
        // Check if email exists
        if (await _userRepository.GetByEmailAsync(registerDto.Email, cancellationToken) != null)
        {
            throw new DomainException($"E-mail '{registerDto.Email}' já está em uso.");
        }
        
        // Hash password and create user
        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);
        var user = UserEntity.Create(registerDto.Username, registerDto.Email, passwordHash, registerDto.Role);
        
        // Save user
        await _userRepository.AddAsync(user, cancellationToken);
        
        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        
        return new AuthResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1),
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }
}