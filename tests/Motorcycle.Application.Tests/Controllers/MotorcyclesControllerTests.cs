using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.API.Controllers;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.Motorcycle;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Motorcycle.Application.Tests.Controllers;

public class MotorcyclesControllerTests
{
    private readonly Mock<IMotorcycleService> _mockService;
    private readonly Mock<ILogger<MotorcyclesController>> _mockLogger;
    private readonly MotorcyclesController _controller;

    public MotorcyclesControllerTests()
    {
        _mockService = new Mock<IMotorcycleService>();
        _mockLogger = new Mock<ILogger<MotorcyclesController>>();
        _controller = new MotorcyclesController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_WithNoFilter_ShouldReturnAllMotorcycles()
    {
        // Arrange
        var motorcycles = new List<MotorcycleDto>
        {
            new MotorcycleDto { Id = Guid.NewGuid(), Model = "Honda CG 160", Year = 2022, LicensePlate = "ABC1234" },
            new MotorcycleDto { Id = Guid.NewGuid(), Model = "Yamaha Factor", Year = 2021, LicensePlate = "XYZ5678" }
        };

        _mockService.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(motorcycles);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMotorcycles = okResult.Value.Should().BeAssignableTo<IEnumerable<MotorcycleResponseModel>>().Subject;
        returnedMotorcycles.Should().HaveCount(motorcycles.Count);
    }

    [Fact]
    public async Task GetAll_WithLicensePlateFilter_ShouldReturnFilteredMotorcycles()
    {
        // Arrange
        var licensePlate = "ABC";
        var motorcycles = new List<MotorcycleDto>
        {
            new MotorcycleDto { Id = Guid.NewGuid(), Model = "Honda CG 160", Year = 2022, LicensePlate = "ABC1234" }
        };

        _mockService.Setup(s => s.SearchByLicensePlateAsync(licensePlate, default))
            .ReturnsAsync(motorcycles);

        // Act
        var result = await _controller.GetAll(licensePlate);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedMotorcycles = okResult.Value.Should().BeAssignableTo<IEnumerable<MotorcycleResponseModel>>().Subject;
        returnedMotorcycles.Should().HaveCount(motorcycles.Count);
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        var id = Guid.NewGuid();
        var motorcycle = new MotorcycleDto { Id = id, Model = "Honda CG 160", Year = 2022, LicensePlate = "ABC1234" };

        _mockService.Setup(s => s.GetByIdAsync(id, default))
            .ReturnsAsync(motorcycle);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<MotorcycleDto>>().Subject;
        response.Data.Should().Be(motorcycle);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.GetByIdAsync(id, default))
            .ReturnsAsync((MotorcycleDto)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedMotorcycle()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto { Model = "Honda CG 160", Year = 2022, LicensePlate = "ABC1234" };
        var createdMotorcycle = new MotorcycleDto { Id = Guid.NewGuid(), Model = "Honda CG 160", Year = 2022, LicensePlate = "ABC1234" };

        _mockService.Setup(s => s.CreateAsync(createDto, default))
            .ReturnsAsync(createdMotorcycle);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(MotorcyclesController.GetById));
        createdResult.RouteValues["id"].Should().Be(createdMotorcycle.Id);
        
        var response = createdResult.Value.Should().BeOfType<ApiResponse<MotorcycleDto>>().Subject;
        response.Data.Should().Be(createdMotorcycle);
    }

    [Fact]
    public async Task Create_WithInvalidData_ShouldHandleDomainException()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto { Model = "", Year = 0, LicensePlate = "INVALID" };
        var errorMessage = "Erro de validação";

        _mockService.Setup(s => s.CreateAsync(createDto, default))
            .ThrowsAsync(new DomainException(errorMessage));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _controller.Create(createDto));
    }
} 