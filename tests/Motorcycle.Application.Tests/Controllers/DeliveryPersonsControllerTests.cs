using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.API.Controllers;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.DeliveryPerson;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Motorcycle.Application.Tests.Controllers;

public class DeliveryPersonsControllerTests
{
    private readonly Mock<IDeliveryPersonService> _mockService;
    private readonly Mock<ILogger<DeliveryPersonsController>> _mockLogger;
    private readonly DeliveryPersonsController _controller;

    public DeliveryPersonsControllerTests()
    {
        _mockService = new Mock<IDeliveryPersonService>();
        _mockLogger = new Mock<ILogger<DeliveryPersonsController>>();
        _controller = new DeliveryPersonsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllDeliveryPersons()
    {
        // Arrange
        var deliveryPersons = new List<DeliveryPersonDto>
        {
            new DeliveryPersonDto { Id = Guid.NewGuid(), Name = "João da Silva" },
            new DeliveryPersonDto { Id = Guid.NewGuid(), Name = "Maria Souza" }
        };

        _mockService.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(deliveryPersons);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<DeliveryPersonDto>>>().Subject;
        response.Data.Should().BeEquivalentTo(deliveryPersons);
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnDeliveryPerson()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deliveryPerson = new DeliveryPersonDto { Id = id, Name = "João da Silva" };

        _mockService.Setup(s => s.GetByIdAsync(id, default))
            .ReturnsAsync(deliveryPerson);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<DeliveryPersonDto>>().Subject;
        response.Data.Should().Be(deliveryPerson);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.GetByIdAsync(id, default))
            .ReturnsAsync((DeliveryPersonDto)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedDeliveryPerson()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto 
        { 
            Name = "João da Silva",
            Cnpj = "04.252.011/0001-10",
            DriverLicense = new DriverLicenseDto
            {
                Number = "12345678901",
                Category = "A",
                ExpirationDate = DateTime.Now.AddYears(5)
            }
        };
        var createdDeliveryPerson = new DeliveryPersonDto { Id = Guid.NewGuid(), Name = "João da Silva" };

        _mockService.Setup(s => s.CreateAsync(createDto, default))
            .ReturnsAsync(createdDeliveryPerson);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(DeliveryPersonsController.GetById));
        createdResult.RouteValues["id"].Should().Be(createdDeliveryPerson.Id);
        
        var response = createdResult.Value.Should().BeOfType<ApiResponse<DeliveryPersonDto>>().Subject;
        response.Data.Should().Be(createdDeliveryPerson);
    }

    [Fact]
    public async Task Create_WithInvalidData_ShouldHandleDomainException()
    {
        // Arrange
        var createDto = new CreateDeliveryPersonDto 
        { 
            Name = "",
            Cnpj = "INVALID"
        };
        var errorMessage = "Erro de validação";

        _mockService.Setup(s => s.CreateAsync(createDto, default))
            .ThrowsAsync(new DomainException(errorMessage));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _controller.Create(createDto));
    }
} 