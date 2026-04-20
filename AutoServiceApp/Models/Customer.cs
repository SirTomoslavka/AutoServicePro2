using System.ComponentModel.DataAnnotations;

namespace AutoServiceApp.Models;

public class Customer
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    public List<Car> Cars { get; set; } = new();

    public string FullName => $"{FirstName} {LastName}";
}