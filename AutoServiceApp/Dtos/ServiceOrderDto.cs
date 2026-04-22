using AutoServiceApp.Models;

namespace AutoServiceApp.Dtos;

public class ServiceOrderDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public ServiceOrderStatus Status { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CarId { get; set; }
    public string? CarBrand { get; set; }
    public string? CarModel { get; set; }
    public string? CarLicensePlate { get; set; }
    public string? CustomerName { get; set; }
    public List<ServiceTaskDto> Tasks { get; set; } = new();
    public List<MechanicSummaryDto> Mechanics { get; set; } = new();
    public InvoiceSummaryDto? Invoice { get; set; }
    public decimal TotalPrice => Tasks.Sum(t => t.TotalPrice);
}

public class ServiceTaskDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid ServiceOrderId { get; set; }
    public List<ServiceTaskPartDto> Parts { get; set; } = new();
    public decimal TotalPrice => Price + Parts.Sum(p => p.TotalPrice);
}

public class ServiceTaskPartDto
{
    public Guid Id { get; set; }
    public Guid ServiceTaskId { get; set; }
    public Guid SparePartId { get; set; }
    public string? SparePartName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}

public class InvoiceSummaryDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}
