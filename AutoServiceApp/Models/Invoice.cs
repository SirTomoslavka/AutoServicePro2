using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoServiceApp.Models;

public class Invoice
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(20)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }

    public bool IsPaid { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public Guid ServiceOrderId { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }
}
