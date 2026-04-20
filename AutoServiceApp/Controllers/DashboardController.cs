using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            CustomersCount = await _db.Customers.CountAsync(),
            CarsCount = await _db.Cars.CountAsync(),
            ServiceOrdersCount = await _db.ServiceOrders.CountAsync(),
            DoneOrdersCount = await _db.ServiceOrders.CountAsync(x => x.Status == ServiceOrderStatus.Done),
            TotalRevenue = await _db.ServiceTasks.SumAsync(x => (decimal?)x.Price) ?? 0
        };

        return View(vm);
    }
}