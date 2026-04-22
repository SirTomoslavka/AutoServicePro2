namespace AutoServiceApp.Dtos;

public class MechanicDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public int OrderCount { get; set; }
    public List<ServiceOrderSummaryDto> Orders { get; set; } = new();
}

public class MechanicSummaryDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
}
