using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoServiceApp.Models;

public class Car
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    [Required]
    [StringLength(20)]
    public string LicensePlate { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public List<ServiceOrder> ServiceOrders { get; set; } = new();

    [NotMapped]
    public string DisplayName => $"{Brand} {Model} ({LicensePlate})";
}