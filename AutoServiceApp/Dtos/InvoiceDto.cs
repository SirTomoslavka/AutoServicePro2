namespace AutoServiceApp.Dtos;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public bool IsPaid { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Note { get; set; }
    public Guid ServiceOrderId { get; set; }
    public string? CustomerName { get; set; }
    public string? CarDisplay { get; set; }
    public List<ServiceTaskDto> Tasks { get; set; } = new();
}
