namespace AutoServiceApp.Dtos;

public class CarDto
{
    public Guid Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string DisplayName => $"{Brand} {Model} ({LicensePlate})";
    public List<ServiceOrderSummaryDto> ServiceOrders { get; set; } = new();
}
