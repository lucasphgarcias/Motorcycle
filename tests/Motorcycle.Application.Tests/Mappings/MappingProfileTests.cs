using AutoMapper;
using FluentAssertions;
using Motorcycle.Application.Mappings;
using Motorcycle.Application.DTOs;
using Motorcycle.Domain.Entities;
using Xunit;

namespace Motorcycle.Application.Tests.Mappings;

public class MappingProfileTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void ShouldMapMotorcycleToMotorcycleDto()
    {
        // Arrange
        var entity = new Domain.Entities.Motorcycle
        {
            Id = Guid.NewGuid(),
            Model = "Model X",
            Brand = "Brand Y",
            Color = "Red",
            ManufacturingYear = 2022,
            PlateNumber = "ABC1234",
            DailyRate = 50.00m,
            IsAvailable = true
        };

        // Act
        var result = _mapper.Map<MotorcycleDto>(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.Model.Should().Be(entity.Model);
        result.Brand.Should().Be(entity.Brand);
        result.Color.Should().Be(entity.Color);
        result.ManufacturingYear.Should().Be(entity.ManufacturingYear);
        result.PlateNumber.Should().Be(entity.PlateNumber);
        result.DailyRate.Should().Be(entity.DailyRate);
        result.IsAvailable.Should().Be(entity.IsAvailable);
    }

    [Fact]
    public void ShouldMapCreateMotorcycleDtoToMotorcycle()
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Model = "Model X",
            Brand = "Brand Y",
            Color = "Red",
            ManufacturingYear = 2022,
            PlateNumber = "ABC1234",
            DailyRate = 50.00m
        };

        // Act
        var result = _mapper.Map<Domain.Entities.Motorcycle>(dto);

        // Assert
        result.Should().NotBeNull();
        result.Model.Should().Be(dto.Model);
        result.Brand.Should().Be(dto.Brand);
        result.Color.Should().Be(dto.Color);
        result.ManufacturingYear.Should().Be(dto.ManufacturingYear);
        result.PlateNumber.Should().Be(dto.PlateNumber);
        result.DailyRate.Should().Be(dto.DailyRate);
        result.IsAvailable.Should().BeTrue();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void ShouldMapRentalToRentalDto()
    {
        // Arrange
        var entity = new Rental
        {
            Id = Guid.NewGuid(),
            MotorcycleId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            DeliveryPersonId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3),
            Status = Domain.Enums.RentalStatus.Pending,
            TotalCost = 150.00m
        };

        // Act
        var result = _mapper.Map<RentalDto>(entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.MotorcycleId.Should().Be(entity.MotorcycleId);
        result.UserId.Should().Be(entity.UserId);
        result.DeliveryPersonId.Should().Be(entity.DeliveryPersonId);
        result.StartDate.Should().Be(entity.StartDate);
        result.EndDate.Should().Be(entity.EndDate);
        result.Status.Should().Be(entity.Status);
        result.TotalCost.Should().Be(entity.TotalCost);
    }

    [Fact]
    public void ShouldMapCreateRentalDtoToRental()
    {
        // Arrange
        var dto = new CreateRentalDto
        {
            MotorcycleId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        // Act
        var result = _mapper.Map<Rental>(dto);

        // Assert
        result.Should().NotBeNull();
        result.MotorcycleId.Should().Be(dto.MotorcycleId);
        result.StartDate.Should().Be(dto.StartDate);
        result.EndDate.Should().Be(dto.EndDate);
        result.Status.Should().Be(Domain.Enums.RentalStatus.Pending);
        result.Id.Should().NotBeEmpty();
    }
} 