using Motorcycle.Domain.Enums;

namespace Motorcycle.Application.DTOs.Rental;

public class RentalDto
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly ExpectedEndDate { get; set; }
    public DateOnly? ActualEndDate { get; set; }
    public RentalPlanType PlanType { get; set; }
    public decimal DailyRate { get; set; }
    public decimal? TotalAmount { get; set; }
    
    // Dados complementares
    public string MotorcycleModel { get; set; } = string.Empty;
    public string MotorcycleLicensePlate { get; set; } = string.Empty;
    public string DeliveryPersonName { get; set; } = string.Empty;
}