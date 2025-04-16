using Motorcycle.Domain.Enums;

namespace Motorcycle.Application.DTOs.Rental;

public class CreateRentalDto
{
    public Guid MotorcycleId { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public DateOnly StartDate { get; set; }
    public RentalPlanType PlanType { get; set; }
}