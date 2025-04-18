using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Infrastructure.Data.Context;
using Motorcycle.Infrastructure.Data.Repositories;
using FluentAssertions;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Motorcycle.Infrastructure.Tests.Repositories;

public class MotorcycleRepositoryTests
{
    private MotorcycleDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<MotorcycleDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new MotorcycleDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddAsync_ShouldAddMotorcycle()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        var motorcycle = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");

        // Act
        await repository.AddAsync(motorcycle);

        // Assert
        var savedMotorcycle = await context.Motorcycles.FirstOrDefaultAsync(m => m.Id == motorcycle.Id);
        savedMotorcycle.Should().NotBeNull();
        savedMotorcycle!.Model.Should().Be("Honda CG 160");
        savedMotorcycle.Year.Should().Be(2023);
        savedMotorcycle.LicensePlate.Value.Should().Be("ABC1234");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnMotorcycle()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        var motorcycle = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
        await repository.AddAsync(motorcycle);

        // Act
        var retrievedMotorcycle = await repository.GetByIdAsync(motorcycle.Id);

        // Assert
        retrievedMotorcycle.Should().NotBeNull();
        retrievedMotorcycle!.Id.Should().Be(motorcycle.Id);
        retrievedMotorcycle.Model.Should().Be(motorcycle.Model);
        retrievedMotorcycle.Year.Should().Be(motorcycle.Year);
        retrievedMotorcycle.LicensePlate.Value.Should().Be(motorcycle.LicensePlate.Value);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMotorcycles()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        
        var motorcycle1 = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
        var motorcycle2 = MotorcycleEntity.Create("Yamaha Factor", 2022, "XYZ5678");
        
        await repository.AddAsync(motorcycle1);
        await repository.AddAsync(motorcycle2);

        // Act
        var motorcycles = (await repository.GetAllAsync()).ToList();

        // Assert
        motorcycles.Should().HaveCount(2);
        motorcycles.Should().Contain(m => m.Id == motorcycle1.Id);
        motorcycles.Should().Contain(m => m.Id == motorcycle2.Id);
    }

    [Fact]
    public async Task SearchByLicensePlateAsync_ShouldReturnMatchingMotorcycles()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        
        var motorcycle1 = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
        var motorcycle2 = MotorcycleEntity.Create("Yamaha Factor", 2022, "ABD5678");
        var motorcycle3 = MotorcycleEntity.Create("Suzuki", 2021, "XYZ9876");
        
        await repository.AddAsync(motorcycle1);
        await repository.AddAsync(motorcycle2);
        await repository.AddAsync(motorcycle3);

        // Act
        var results = (await repository.SearchByLicensePlateAsync("AB")).ToList();

        // Assert
        results.Should().HaveCount(2);
        results.Should().Contain(m => m.LicensePlate.Value == "ABC1234");
        results.Should().Contain(m => m.LicensePlate.Value == "ABD5678");
        results.Should().NotContain(m => m.LicensePlate.Value == "XYZ9876");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMotorcycle()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        var motorcycle = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
        await repository.AddAsync(motorcycle);
        
        // Importante: desanexar o contexto para evitar problemas de rastreamento
        context.ChangeTracker.Clear();

        // Criar uma nova instância com o mesmo ID para simular uma atualização
        var updatedMotorcycle = MotorcycleEntity.Create("Honda Biz", 2022, "XYZ5678");
        
        // Update using reflection to set the Id (simulating the real update scenario)
        var idProperty = typeof(Entity).GetProperty("Id");
        idProperty?.SetValue(updatedMotorcycle, motorcycle.Id);

        // Act
        await repository.UpdateAsync(updatedMotorcycle);

        // Assert
        var retrievedMotorcycle = await context.Motorcycles.FindAsync(motorcycle.Id);
        retrievedMotorcycle.Should().NotBeNull();
        retrievedMotorcycle!.Model.Should().Be("Honda Biz");
        retrievedMotorcycle.Year.Should().Be(2022);
        retrievedMotorcycle.LicensePlate.Value.Should().Be("XYZ5678");
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveMotorcycle()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var repository = new MotorcycleRepository(context);
        var motorcycle = MotorcycleEntity.Create("Honda CG 160", 2023, "ABC1234");
        await repository.AddAsync(motorcycle);

        // Act
        await repository.RemoveAsync(motorcycle);

        // Assert
        var retrievedMotorcycle = await context.Motorcycles.FindAsync(motorcycle.Id);
        retrievedMotorcycle.Should().BeNull();
    }
} 