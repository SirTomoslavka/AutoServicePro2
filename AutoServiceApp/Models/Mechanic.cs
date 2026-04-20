using System.ComponentModel.DataAnnotations;

namespace AutoServiceApp.Models;

public class Mechanic
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Specialization { get; set; }

    public List<ServiceOrderMechanic> ServiceOrderMechanics { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}";
}