using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoServiceApp.Models;

public class ServiceTaskPart
{
    [Key]
    public Guid Id { get; set; }

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public Guid ServiceTaskId { get; set; }
    public ServiceTask? ServiceTask { get; set; }

    public Guid SparePartId { get; set; }
    public SparePart? SparePart { get; set; }

    [NotMapped]
    public decimal TotalPrice => Quantity * UnitPrice;
}
