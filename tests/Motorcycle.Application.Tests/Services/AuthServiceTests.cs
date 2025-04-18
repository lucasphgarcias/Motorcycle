using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.Application.DTOs.Auth;
using Motorcycle.Application.Services;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Motorcycle.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtService> _mockJwtService;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtService = new Mock<IJwtService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        _service = new AuthService(
            _mockUserRepository.Object,
            _mockJwtService.Object,
            _mockPasswordHasher.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "Password123!"
        };

        var user = UserEntity.Create(
            "testuser",
            "test@example.com",
            "hashedpassword",
            "User");

        var token = "jwt-token";
        var expectedResponse = new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString(),
            Expiration = It.IsAny<DateTime>()
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(loginDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            .Returns(true);
        _mockJwtService.Setup(jwtService => jwtService.GenerateToken(user))
            .Returns(token);

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.Username.Should().Be(user.Username);
        result.Role.Should().Be(user.Role.ToString());
        _mockUserRepository.Verify(repo => repo.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ShouldThrowDomainException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "nonexistentuser",
            Password = "Password123!"
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(loginDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.LoginAsync(loginDto));
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowDomainException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "WrongPassword123!"
        };

        var user = UserEntity.Create(
            "testuser",
            "test@example.com",
            "hashedpassword",
            "User");

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(loginDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(user.PasswordHash, loginDto.Password))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.LoginAsync(loginDto));
        _mockUserRepository.Verify(repo => repo.UpdateAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldRegisterUserAndReturnToken()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = "User"
        };

        var passwordHash = "hashedpassword";
        var token = "jwt-token";
        var expectedResponse = new AuthResponseDto
        {
            Token = token,
            Username = registerDto.Username,
            Role = registerDto.Role,
            Expiration = It.IsAny<DateTime>()
        };

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(registerDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(registerDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);
        _mockPasswordHasher.Setup(hasher => hasher.HashPassword(registerDto.Password))
            .Returns(passwordHash);
        _mockJwtService.Setup(jwtService => jwtService.GenerateToken(It.IsAny<UserEntity>()))
            .Returns(token);

        // Act
        var result = await _service.RegisterAsync(registerDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.Username.Should().Be(registerDto.Username);
        result.Role.Should().Be(registerDto.Role);
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithPasswordMismatch_ShouldThrowDomainException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "newuser@example.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!",
            Role = "User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.RegisterAsync(registerDto));
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingUsername_ShouldThrowDomainException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "existinguser",
            Email = "newuser@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = "User"
        };

        var existingUser = UserEntity.Create(
            "existinguser",
            "existing@example.com",
            "hashedpassword",
            "User");

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(registerDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.RegisterAsync(registerDto));
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowDomainException()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Role = "User"
        };

        var existingUser = UserEntity.Create(
            "existinguser",
            "existing@example.com",
            "hashedpassword",
            "User");

        _mockUserRepository.Setup(repo => repo.GetByUsernameAsync(registerDto.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserEntity)null);
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(registerDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.RegisterAsync(registerDto));
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<UserEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
} 