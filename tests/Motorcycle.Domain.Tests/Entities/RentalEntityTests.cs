using FluentAssertions;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;
using Xunit;

namespace Motorcycle.Domain.Tests.Entities;

public class RentalEntityTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateRental()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var planType = RentalPlanType.SevenDays;
        var deliveryPerson = CreateValidDeliveryPerson();

        // Act
        var rental = RentalEntity.Create(
            motorcycleId,
            deliveryPersonId,
            startDate,
            planType,
            deliveryPerson);

        // Assert
        rental.Should().NotBeNull();
        rental.MotorcycleId.Should().Be(motorcycleId);
        rental.DeliveryPersonId.Should().Be(deliveryPersonId);
        rental.Period.StartDate.Should().Be(startDate);
        rental.Period.PlanType.Should().Be(planType);
        rental.TotalAmount.Should().BeNull();
    }
    
    [Fact]
    public void Create_WithInvalidDeliveryPerson_ShouldThrowDomainException()
    {
        // Arrange
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var planType = RentalPlanType.SevenDays;
        
        // Criar entregador sem habilitação para motocicletas
        var deliveryPerson = DeliveryPersonEntity.Create(
            "João da Silva",
            "04.252.011/0001-10",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            "12345678901",
            LicenseType.B // Tipo B não permite dirigir motos
        );

        // Act & Assert
        Assert.Throws<DomainException>(() => RentalEntity.Create(
            motorcycleId,
            deliveryPersonId,
            startDate,
            planType,
            deliveryPerson));
    }
    
    [Fact]
    public void ReturnMotorcycle_WithValidDate_ShouldFinalizeRental()
    {
        // Arrange
        var rental = CreateValidRental();
        var returnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)); // Dentro do prazo do plano
        
        // Act
        rental.ReturnMotorcycle(returnDate);
        
        // Assert
        rental.TotalAmount.Should().NotBeNull();
        rental.Period.ActualEndDate.Should().Be(returnDate);
    }
    
    [Fact]
    public void ReturnMotorcycle_WhenAlreadyFinalized_ShouldThrowDomainException()
    {
        // Arrange
        var rental = CreateValidRental();
        var returnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7));
        
        // Finalizar o aluguel uma vez
        rental.ReturnMotorcycle(returnDate);
        
        // Act & Assert
        Assert.Throws<DomainException>(() => 
            rental.ReturnMotorcycle(DateOnly.FromDateTime(DateTime.Today.AddDays(8))));
    }
    
    [Fact]
    public void CalculateTotalAmount_WithEarlyReturn_ShouldIncludePenalty()
    {
        // Arrange
        var rental = CreateValidRental();
        var earlyReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)); // 3 dias para um plano de 7 dias
        
        // Act
        rental.ReturnMotorcycle(earlyReturnDate);
        
        // Assert
        rental.TotalAmount.Should().NotBeNull();
        // Verifica se o valor total inclui alguma penalidade
        rental.TotalAmount!.Amount.Should().BeGreaterThan(rental.DailyRate.Multiply(3).Amount);
    }
    
    [Fact]
    public void CalculateTotalAmount_WithLateReturn_ShouldIncludeExtraFee()
    {
        // Arrange
        var rental = CreateValidRental();
        var lateReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)); // 10 dias para um plano de 7 dias
        
        // Act
        rental.ReturnMotorcycle(lateReturnDate);
        
        // Assert
        rental.TotalAmount.Should().NotBeNull();
        // Base: 7 dias x R$30 = R$210 + taxa por 3 dias extras (3 x R$50 = R$150) = R$360
        rental.TotalAmount!.Amount.Should().Be(360m);
    }
    
    private RentalEntity CreateValidRental()
    {
        var motorcycleId = Guid.NewGuid();
        var deliveryPersonId = Guid.NewGuid();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var deliveryPerson = CreateValidDeliveryPerson();
        
        return RentalEntity.Create(
            motorcycleId,
            deliveryPersonId,
            startDate,
            planType,
            deliveryPerson);
    }
    
    private DeliveryPersonEntity CreateValidDeliveryPerson()
    {
        return DeliveryPersonEntity.Create(
            "João da Silva",
            "04.252.011/0001-10",
            DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
            "12345678901",
            LicenseType.A // Tipo A permite dirigir motos
        );
    }
} 