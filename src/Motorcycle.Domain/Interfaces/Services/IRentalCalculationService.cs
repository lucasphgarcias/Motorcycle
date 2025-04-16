using Motorcycle.Domain.Entities;
using Motorcycle.Domain.Enums;
using Motorcycle.Domain.ValueObjects;

namespace Motorcycle.Domain.Interfaces.Services;

public interface IRentalCalculationService
{
    Money CalculateDailyRate(RentalPlanType planType);
    Money CalculateTotalAmount(RentalEntity rental);
    Money CalculateEarlyReturnPenalty(RentalPlanType planType, int unusedDays, Money dailyRate);
    Money CalculateLateReturnFee(int extraDays);
}