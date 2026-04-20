using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardViewModel> GetStatsAsync() => new DashboardViewModel
    {
        CustomersCount = await _db.Customers.CountAsync(),
        CarsCount = await _db.Cars.CountAsync(),
        ServiceOrdersCount = await _db.ServiceOrders.CountAsync(),
        DoneOrdersCount = await _db.ServiceOrders.CountAsync(x => x.Status == ServiceOrderStatus.Done),
        TotalRevenue = await _db.Invoices.SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
        PaidRevenue = await _db.Invoices.Where(x => x.IsPaid).SumAsync(x => (decimal?)x.TotalAmount) ?? 0
    };
}
