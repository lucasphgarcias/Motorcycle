using Motorcycle.Domain.Enums;
using Motorcycle.Domain.Exceptions;
using Motorcycle.Domain.ValueObjects;

namespace Motorcycle.Domain.Entities;

public class RentalEntity : Entity
{
    public Guid MotorcycleId { get; private set; }
    public Guid DeliveryPersonId { get; private set; }
    public required RentalPeriod Period { get; set; }
    public required Money DailyRate { get; set; }
    public Money? TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Propriedades de navegação para EF Core
    public MotorcycleEntity Motorcycle { get; private set; } = null!;
    public DeliveryPersonEntity DeliveryPerson { get; private set; } = null!;

    // Para EF Core
    private RentalEntity() : base() { }

    private RentalEntity(
        Guid motorcycleId,
        Guid deliveryPersonId,
        RentalPeriod period,
        Money dailyRate)
        : base()
    {
        MotorcycleId = motorcycleId;
        DeliveryPersonId = deliveryPersonId;
        CreatedAt = DateTime.UtcNow;
        Period = period;
        DailyRate = dailyRate;
    }

    public static RentalEntity Create(
        Guid motorcycleId,
        Guid deliveryPersonId,
        DateOnly startDate,
        RentalPlanType planType,
        DeliveryPersonEntity deliveryPerson)
    {
        // Verifica se o entregador pode dirigir motocicletas
        if (!deliveryPerson.CanRentMotorcycle())
            throw new DomainException("O entregador não está habilitado para conduzir motocicletas.");

        var period = RentalPeriod.Create(startDate, planType);
        var dailyRate = GetDailyRateForPlan(planType);

        return new RentalEntity(motorcycleId, deliveryPersonId, period, dailyRate)
        {
            Period = period,
            DailyRate = dailyRate
        };
    }

    public void ReturnMotorcycle(DateOnly returnDate)
    {
        if (TotalAmount != null)
            throw new DomainException("Esta locação já foi finalizada.");

        Period.SetActualEndDate(returnDate);
        TotalAmount = CalculateTotalAmount();
    }

    public Money CalculateTotalAmount()
    {
        if (!Period.ActualEndDate.HasValue)
            throw new DomainException("A data de devolução deve ser informada para calcular o valor total.");

        // Valor base considerando as diárias utilizadas
        var actualDays = Period.CalculateRentalDays();
        var baseAmount = DailyRate.Multiply(actualDays);

        // Se devolveu antes do prazo previsto
        if (Period.IsEarlyReturn())
        {
            var unusedDays = Period.CalculateUnusedDays();
            var unusedAmount = DailyRate.Multiply(unusedDays);
            var penaltyPercentage = GetEarlyReturnPenaltyPercentage(Period.PlanType);
            var penaltyAmount = unusedAmount.Multiply(penaltyPercentage);
            
            // Soma base + multa
            return baseAmount.Add(penaltyAmount);
        }
        
        // Se devolveu após o prazo previsto
        if (Period.IsLateReturn())
        {
            var extraDays = Period.CalculateExtraDays();
            var extraDailyRate = Money.Create(50m); // Taxa fixa de R$50,00 por dia adicional
            var extraAmount = extraDailyRate.Multiply(extraDays);
            
            // Soma base + valor dos dias extras
            return baseAmount.Add(extraAmount);
        }
        
        // Se devolveu na data prevista
        return baseAmount;
    }

    private static Money GetDailyRateForPlan(RentalPlanType planType)
    {
        return planType switch
        {
            RentalPlanType.SevenDays => Money.Create(30m),
            RentalPlanType.FifteenDays => Money.Create(28m),
            RentalPlanType.ThirtyDays => Money.Create(22m),
            RentalPlanType.FortyFiveDays => Money.Create(20m),
            RentalPlanType.FiftyDays => Money.Create(18m),
            _ => throw new DomainException("Plano de aluguel inválido.")
        };
    }

    private static decimal GetEarlyReturnPenaltyPercentage(RentalPlanType planType)
    {
        return planType switch
        {
            RentalPlanType.SevenDays => 0.2m, // 20%
            RentalPlanType.FifteenDays => 0.4m, // 40%
            _ => 0m // Outros planos não têm multa
        };
    }
}