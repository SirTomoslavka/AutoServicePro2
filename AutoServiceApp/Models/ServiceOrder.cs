using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoServiceApp.Models;

public class ServiceOrder
{
    [Key]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.New;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public Guid CarId { get; set; }
    public Car? Car { get; set; }

    public List<ServiceTask> Tasks { get; set; } = new();
    public List<ServiceOrderMechanic> ServiceOrderMechanics { get; set; } = new();
    public Invoice? Invoice { get; set; }

    [NotMapped]
    public decimal TotalPrice => Tasks.Sum(t => t.Price + t.ServiceTaskParts.Sum(p => p.Quantity * p.UnitPrice));
}