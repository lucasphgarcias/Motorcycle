using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Motorcycle.API.Controllers;
using Motorcycle.API.Models.Responses;
using Motorcycle.Application.DTOs.Rental;
using Motorcycle.Application.Interfaces;
using Motorcycle.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Motorcycle.Application.Tests.Controllers;

public class RentalsControllerTests
{
    private readonly Mock<IRentalService> _mockService;
    private readonly Mock<ILogger<RentalsController>> _mockLogger;
    private readonly RentalsController _controller;

    public RentalsControllerTests()
    {
        _mockService = new Mock<IRentalService>();
        _mockLogger = new Mock<ILogger<RentalsController>>();
        _controller = new RentalsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllRentals()
    {
        // Arrange
        var rentals = new List<RentalDto>
        {
            new RentalDto { Id = Guid.NewGuid() },
            new RentalDto { Id = Guid.NewGuid() }
        };

        _mockService.Setup(s => s.GetAllAsync(default))
            .ReturnsAsync(rentals);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<RentalDto>>>().Subject;
        response.Data.Should().BeEquivalentTo(rentals);
    }

    [Fact]
    public async Task GetById_WithExistingId_ShouldReturnRental()
    {
        // Arrange
        var id = Guid.NewGuid();
        var rental = new RentalDto { Id = id };

        _mockService.Setup(s => s.GetByIdAsync(id, default))
            .ReturnsAsync(rental);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<RentalDto>>().Subject;
        response.Data.Should().Be(rental);
    }

    [Fact]
    public async Task GetById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.GetByIdAsync(id, default))
            .ReturnsAsync((RentalDto)null);

        // Act
        var result = await _controller.GetById(id);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnCreatedRental()
    {
        // Arrange
        var createDto = new CreateRentalDto 
        { 
            MotorcycleId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid()
        };
        var createdRental = new RentalDto { Id = Guid.NewGuid() };

        _mockService.Setup(s => s.CreateAsync(createDto, default))
            .ReturnsAsync(createdRental);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(RentalsController.GetById));
        createdResult.RouteValues["id"].Should().Be(createdRental.Id);
        
        var response = createdResult.Value.Should().BeOfType<ApiResponse<RentalDto>>().Subject;
        response.Data.Should().Be(createdRental);
    }

    [Fact]
    public async Task ReturnMotorcycle_WithValidData_ShouldReturnTotalAmount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDto = new ReturnMotorcycleDto 
        { 
            ReturnDate = DateOnly.FromDateTime(DateTime.Today)
        };
        var totalAmount = new RentalTotalAmountDto { TotalAmount = 300 };

        _mockService.Setup(s => s.ReturnMotorcycleAsync(id, returnDto, default))
            .ReturnsAsync(totalAmount);

        // Act
        var result = await _controller.ReturnMotorcycle(id, returnDto);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<RentalTotalAmountDto>>().Subject;
        response.Data.Should().Be(totalAmount);
    }

    [Fact]
    public async Task CalculateTotalAmount_WithValidData_ShouldReturnCalculatedAmount()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDate = DateOnly.FromDateTime(DateTime.Today);
        var totalAmount = new RentalTotalAmountDto { TotalAmount = 300 };

        _mockService.Setup(s => s.CalculateTotalAmountAsync(id, returnDate, default))
            .ReturnsAsync(totalAmount);

        // Act
        var result = await _controller.CalculateTotalAmount(id, returnDate);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<RentalTotalAmountDto>>().Subject;
        response.Data.Should().Be(totalAmount);
    }

    [Fact]
    public async Task CalculateTotalAmount_WithNonExistingId_ShouldHandleDomainException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var returnDate = DateOnly.FromDateTime(DateTime.Today);
        var errorMessage = "Aluguel não encontrado";

        _mockService.Setup(s => s.CalculateTotalAmountAsync(id, returnDate, default))
            .ThrowsAsync(new DomainException(errorMessage));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _controller.CalculateTotalAmount(id, returnDate));
    }
} 