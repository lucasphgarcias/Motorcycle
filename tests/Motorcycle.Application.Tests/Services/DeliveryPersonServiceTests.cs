using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Application.Services;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;
using Motorcycle.Domain.ValueObjects;
using System.Text;
using Xunit;

namespace Motorcycle.Application.Tests.Services;

public class DeliveryPersonServiceTests
{
    private readonly Mock<IDeliveryPersonRepository> _mockRepository;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateDeliveryPersonDto>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateDriverLicenseImageDto>> _mockImageValidator;
    private readonly Mock<ILogger<DeliveryPersonService>> _mockLogger;
    private readonly DeliveryPersonService _service;

    public DeliveryPersonServiceTests()
    {
        _mockRepository = new Mock<IDeliveryPersonRepository>();
        _mockFileStorageService = new Mock<IFileStorageService>();
        _mockMapper = new Mock<IMapper>();
        _mockCreateValidator = new Mock<IValidator<CreateDeliveryPersonDto>>();
        _mockImageValidator = new Mock<IValidator<UpdateDriverLicenseImageDto>>();
        _mockLogger = new Mock<ILogger<DeliveryPersonService>>();

        _service = new DeliveryPersonService(
            _mockRepository.Object,
            _mockFileStorageService.Object,
            _mockMapper.Object,
            _mockCreateValidator.Object,
            _mockImageValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDeliveryPersons()
    {
        // Arrange
        var deliveryPersons = new List<DeliveryPersonEntity> 
        { 
            CreateSampleDeliveryPerson(),
            CreateSampleDeliveryPerson()
        };
        var dtos = new List<DeliveryPersonDto> 
        { 
            new DeliveryPersonDto { Id = Guid.NewGuid() },
            new DeliveryPersonDto { Id = Guid.NewGuid() }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPersons);
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<DeliveryPersonDto>>(deliveryPersons))
            .Returns(dtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(dtos);
        _mockRepository.Verify(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPerson = CreateSampleDeliveryPerson();
        var dto = new DeliveryPersonDto { Id = id };

        _mockRepository.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);
        _mockMapper.Setup(mapper => mapper.Map<DeliveryPersonDto>(deliveryPerson))
            .Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeEquivalentTo(dto);
        _mockRepository.Verify(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPersonEntity)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCnpjAsync_WithExistingCnpj_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var cnpj = "12345678000190";
        var deliveryPerson = CreateSampleDeliveryPerson();
        var dto = new DeliveryPersonDto { Cnpj = cnpj };

        _mockRepository.Setup(repo => repo.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);
        _mockMapper.Setup(mapper => mapper.Map<DeliveryPersonDto>(deliveryPerson))
            .Returns(dto);

        // Act
        var result = await _service.GetByCnpjAsync(cnpj);

        // Assert
        result.Should().BeEquivalentTo(dto);
        _mockRepository.Verify(repo => repo.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateDeliveryPerson()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto
        {
            Name = "Test Company",
            Cnpj = "12345678000190",
            DriverLicense = new DriverLicenseDto
            {
                Number = "12345678901",
                Category = "A",
                ExpirationDate = DateTime.Now.AddYears(5)
            },
            PhoneNumber = "11987654321",
            Email = "test@test.com",
            Address = "Test Street, 123"
        };

        var validationResult = new ValidationResult();
        var deliveryPerson = CreateSampleDeliveryPerson();
        var resultDto = new DeliveryPersonDto { Id = Guid.NewGuid(), Name = "Test Company" };

        _mockCreateValidator.Setup(validator => validator.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockMapper.Setup(mapper => mapper.Map<DeliveryPersonEntity>(createDto))
            .Returns(deliveryPerson);
        _mockMapper.Setup(mapper => mapper.Map<DeliveryPersonDto>(deliveryPerson))
            .Returns(resultDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().BeEquivalentTo(resultDto);
        _mockRepository.Verify(repo => repo.AddAsync(deliveryPerson, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldThrowDomainException()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto
        {
            // Missing required fields
            Name = "",
            Cnpj = "invalid-cnpj"
        };

        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Cnpj", "Invalid CNPJ format")
        });

        _mockCreateValidator.Setup(validator => validator.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(createDto));
        _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<DeliveryPersonEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateDriverLicenseImageAsync_WithValidImage_ShouldUpdateImage()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPerson = CreateSampleDeliveryPerson();
        var mockFile = new Mock<IFormFile>();
        var content = "Hello World from a Fake File";
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.Length).Returns(ms.Length);
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");

        var validationResult = new ValidationResult();
        var resultDto = new DeliveryPersonDto { Id = id };
        var uploadedPath = $"licenses/{id}/{fileName}";

        _mockRepository.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);
        _mockImageValidator.Setup(validator => validator.ValidateAsync(It.IsAny<UpdateDriverLicenseImageDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _mockFileStorageService.Setup(fs => fs.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(uploadedPath);
        _mockMapper.Setup(mapper => mapper.Map<DeliveryPersonDto>(deliveryPerson))
            .Returns(resultDto);

        // Act
        var result = await _service.UpdateDriverLicenseImageAsync(id, mockFile.Object);

        // Assert
        result.Should().BeEquivalentTo(resultDto);
        _mockRepository.Verify(repo => repo.UpdateAsync(deliveryPerson, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPerson = CreateSampleDeliveryPerson();

        _mockRepository.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deliveryPerson);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(repo => repo.RemoveAsync(deliveryPerson, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DeliveryPersonEntity)null);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(repo => repo.RemoveAsync(It.IsAny<DeliveryPersonEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private DeliveryPersonEntity CreateSampleDeliveryPerson()
    {
        return DeliveryPersonEntity.Create(
            "Test Company",
            Cnpj.Create("12345678000190"),
            DriverLicense.Create("12345678901", "A", DateTime.Now.AddYears(5)),
            "11987654321",
            "test@test.com",
            "Test Street, 123",
            null
        );
    }
} 