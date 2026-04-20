namespace AutoServiceApp.ViewModels;

public class DashboardViewModel
{
    public int CustomersCount { get; set; }
    public int CarsCount { get; set; }
    public int ServiceOrdersCount { get; set; }
    public int DoneOrdersCount { get; set; }
    public decimal TotalRevenue { get; set; }
}