using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;

namespace Motorcycle.Domain.ValueObjects;

public class RentalPeriod : IEquatable<RentalPeriod>
{
    public DateOnly StartDate { get; }
    public DateOnly ExpectedEndDate { get; }
    public DateOnly? ActualEndDate { get; private set; }
    public RentalPlanType PlanType { get; }

    private RentalPeriod(DateOnly startDate, RentalPlanType planType, DateOnly? actualEndDate = null)
    {
        StartDate = startDate;
        PlanType = planType;
        ExpectedEndDate = CalculateExpectedEndDate(startDate, planType);
        ActualEndDate = actualEndDate;
    }

    public static RentalPeriod Create(DateOnly startDate, RentalPlanType planType)
    {
        ValidateStartDate(startDate);
        return new RentalPeriod(startDate, planType);
    }

    private static void ValidateStartDate(DateOnly startDate)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        // Garante que a data de início é, no mínimo, o dia seguinte
        if (startDate <= today)
            throw new DomainException("A data de início deve ser, no mínimo, o dia seguinte à data atual.");
    }

    private static DateOnly CalculateExpectedEndDate(DateOnly startDate, RentalPlanType planType)
    {
        return startDate.AddDays((int)planType - 1); // -1 porque o período inclui o dia inicial
    }

    public void SetActualEndDate(DateOnly endDate)
    {
        if (endDate < StartDate)
            throw new DomainException("A data de término não pode ser anterior à data de início.");

        ActualEndDate = endDate;
    }

    public int CalculateRentalDays()
    {
        if (!ActualEndDate.HasValue)
            return (int)PlanType; // Retorna a duração do plano se não houver data de término real
            
        // Calcula a diferença em dias
        var rentalDays = ActualEndDate.Value.DayNumber - StartDate.DayNumber + 1; // +1 para incluir o dia inicial
        return rentalDays;
    }

    public bool IsEarlyReturn()
    {
        return ActualEndDate.HasValue && ActualEndDate.Value < ExpectedEndDate;
    }

    public bool IsLateReturn()
    {
        return ActualEndDate.HasValue && ActualEndDate.Value > ExpectedEndDate;
    }

    public int CalculateUnusedDays()
    {
        if (!IsEarlyReturn())
            return 0;
            
        return ExpectedEndDate.DayNumber - ActualEndDate!.Value.DayNumber;
    }

    public int CalculateExtraDays()
    {
        if (!IsLateReturn())
            return 0;
            
        return ActualEndDate!.Value.DayNumber - ExpectedEndDate.DayNumber;
    }

    public bool Equals(RentalPeriod? other)
    {
        if (other is null)
            return false;

        return StartDate == other.StartDate && 
               ExpectedEndDate == other.ExpectedEndDate && 
               ActualEndDate == other.ActualEndDate && 
               PlanType == other.PlanType;
    }

    public override bool Equals(object? obj)
    {
        return obj is RentalPeriod other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, ExpectedEndDate, ActualEndDate, PlanType);
    }
}