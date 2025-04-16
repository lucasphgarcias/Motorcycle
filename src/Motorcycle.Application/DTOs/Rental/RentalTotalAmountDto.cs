namespace Motorcycle.Application.DTOs.Rental;

public class RentalTotalAmountDto
{
    public Guid RentalId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "BRL";
    public int ActualDays { get; set; }
    public bool IsEarlyReturn { get; set; }
    public bool IsLateReturn { get; set; }
    public int? UnusedDays { get; set; }
    public int? ExtraDays { get; set; }
    public decimal? PenaltyAmount { get; set; }
    public decimal? ExtraAmount { get; set; }
}