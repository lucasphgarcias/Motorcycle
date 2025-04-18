using FluentAssertions;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;
using Xunit;

namespace Motorcycle.Domain.Tests.ValueObjects;

public class RentalPeriodTests
{
    [Fact]
    public void Create_WithValidParams_ShouldCreateRentalPeriod()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var planType = RentalPlanType.SevenDays;

        // Act
        var period = RentalPeriod.Create(startDate, planType);

        // Assert
        period.Should().NotBeNull();
        period.StartDate.Should().Be(startDate);
        period.PlanType.Should().Be(planType);
        period.PlannedEndDate.Should().Be(startDate.AddDays(7));
        period.ActualEndDate.Should().BeNull();
    }

    [Fact]
    public void Create_WithStartDateInPast_ShouldThrowDomainException()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        var planType = RentalPlanType.SevenDays;

        // Act & Assert
        Assert.Throws<DomainException>(() => RentalPeriod.Create(startDate, planType));
    }

    [Fact]
    public void SetActualEndDate_WithValidDate_ShouldSetActualEndDate()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(5);

        // Act
        period.SetActualEndDate(returnDate);

        // Assert
        period.ActualEndDate.Should().Be(returnDate);
    }

    [Fact]
    public void SetActualEndDate_WithDateBeforeStartDate_ShouldThrowDomainException()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(-1);

        // Act & Assert
        Assert.Throws<DomainException>(() => period.SetActualEndDate(returnDate));
    }

    [Fact]
    public void IsEarlyReturn_WhenActualEndDateIsBeforePlannedEndDate_ShouldReturnTrue()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(5);
        period.SetActualEndDate(returnDate);

        // Act
        var result = period.IsEarlyReturn();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsEarlyReturn_WhenActualEndDateIsEqualToPlannedEndDate_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(7);
        period.SetActualEndDate(returnDate);

        // Act
        var result = period.IsEarlyReturn();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsEarlyReturn_WhenActualEndDateIsAfterPlannedEndDate_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(10);
        period.SetActualEndDate(returnDate);

        // Act
        var result = period.IsEarlyReturn();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsLateReturn_WhenActualEndDateIsAfterPlannedEndDate_ShouldReturnTrue()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(10);
        period.SetActualEndDate(returnDate);

        // Act
        var result = period.IsLateReturn();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsLateReturn_WhenActualEndDateIsEqualToPlannedEndDate_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(7);
        period.SetActualEndDate(returnDate);

        // Act
        var result = period.IsLateReturn();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsLateReturn_WhenActualEndDateIsBeforePlannedEndDate_ShouldReturnFalse()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(5);
        period.SetActualEndDate(returnDate);

        // Act
        var result = period.IsLateReturn();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CalculateRentalDays_WhenActualEndDateIsSet_ShouldReturnCorrectDaysCount()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(5);
        period.SetActualEndDate(returnDate);

        // Act
        var days = period.CalculateRentalDays();

        // Assert
        days.Should().Be(6); // Including start and end date (5-0+1=6)
    }

    [Fact]
    public void CalculateRentalDays_WhenActualEndDateIsNotSet_ShouldThrowDomainException()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);

        // Act & Assert
        Assert.Throws<DomainException>(() => period.CalculateRentalDays());
    }

    [Fact]
    public void CalculateUnusedDays_WhenIsEarlyReturn_ShouldReturnCorrectDaysCount()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(4);
        period.SetActualEndDate(returnDate);

        // Act
        var unusedDays = period.CalculateUnusedDays();

        // Assert
        unusedDays.Should().Be(2); // 7 - 5 = 2 (7 days plan, 5 days used including start and end)
    }

    [Fact]
    public void CalculateUnusedDays_WhenNotEarlyReturn_ShouldThrowDomainException()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(7);
        period.SetActualEndDate(returnDate);

        // Act & Assert
        Assert.Throws<DomainException>(() => period.CalculateUnusedDays());
    }

    [Fact]
    public void CalculateExtraDays_WhenIsLateReturn_ShouldReturnCorrectDaysCount()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(9);
        period.SetActualEndDate(returnDate);

        // Act
        var extraDays = period.CalculateExtraDays();

        // Assert
        extraDays.Should().Be(3); // 10 - 7 = 3 (10 days used, 7 days plan)
    }

    [Fact]
    public void CalculateExtraDays_WhenNotLateReturn_ShouldThrowDomainException()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var planType = RentalPlanType.SevenDays;
        var period = RentalPeriod.Create(startDate, planType);
        var returnDate = startDate.AddDays(5);
        period.SetActualEndDate(returnDate);

        // Act & Assert
        Assert.Throws<DomainException>(() => period.CalculateExtraDays());
    }

    [Theory]
    [InlineData(RentalPlanType.SevenDays, 7)]
    [InlineData(RentalPlanType.FifteenDays, 15)]
    [InlineData(RentalPlanType.ThirtyDays, 30)]
    public void GetPlannedDurationInDays_ShouldReturnCorrectDaysForPlanType(RentalPlanType planType, int expectedDays)
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var period = RentalPeriod.Create(startDate, planType);

        // Act
        var durationInDays = period.GetPlannedDurationInDays();

        // Assert
        durationInDays.Should().Be(expectedDays);
    }
} 