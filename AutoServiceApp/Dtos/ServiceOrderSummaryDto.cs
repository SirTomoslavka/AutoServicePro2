using AutoServiceApp.Models;

namespace AutoServiceApp.Dtos;

public class ServiceOrderSummaryDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public ServiceOrderStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? CarDisplay { get; set; }
    public string? CustomerName { get; set; }
}
