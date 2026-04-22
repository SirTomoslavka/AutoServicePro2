namespace AutoServiceApp.Dtos;

public class SparePartDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CatalogNumber { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public List<SparePartUsageDto> Usages { get; set; } = new();
}

public class SparePartUsageDto
{
    public string? ServiceTaskName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
