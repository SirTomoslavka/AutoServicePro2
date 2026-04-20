using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class ServiceOrdersControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ServiceOrdersController _controller;

    public ServiceOrdersControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        var serviceOrderService = new ServiceOrderService(_db);
        var carService = new CarService(_db);
        var mechanicService = new MechanicService(_db);
        var sparePartService = new SparePartService(_db);
        _controller = new ServiceOrdersController(serviceOrderService, carService, mechanicService, sparePartService);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Index_ReturnsAllOrders()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "Test", Status = ServiceOrderStatus.New });
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "Test 2", Status = ServiceOrderStatus.Done });
        await _db.SaveChangesAsync();

        var result = await _controller.Index(null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ServiceOrder>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Index_FiltersByStatus()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "Test", Status = ServiceOrderStatus.New });
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "Test 2", Status = ServiceOrderStatus.Done });
        await _db.SaveChangesAsync();

        var result = await _controller.Index(ServiceOrderStatus.Done);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<ServiceOrder>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task ChangeStatus_ChangesOrderStatus()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test", Status = ServiceOrderStatus.New };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.ChangeStatus(order.Id, ServiceOrderStatus.InProgress);

        Assert.IsType<RedirectToActionResult>(result);
        var updated = await _db.ServiceOrders.FindAsync(order.Id);
        Assert.Equal(ServiceOrderStatus.InProgress, updated!.Status);
    }

    [Fact]
    public async Task ChangeStatus_ToDone()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test", Status = ServiceOrderStatus.InProgress };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.ChangeStatus(order.Id, ServiceOrderStatus.Done);

        Assert.IsType<RedirectToActionResult>(result);
        var updated = await _db.ServiceOrders.FindAsync(order.Id);
        Assert.Equal(ServiceOrderStatus.Done, updated!.Status);
    }

    [Fact]
    public async Task ChangeStatus_InvalidId_ReturnsNotFound()
    {
        var result = await _controller.ChangeStatus(Guid.NewGuid(), ServiceOrderStatus.Done);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Detail_WithValidId_ReturnsView()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test", Status = ServiceOrderStatus.New };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.Detail(order.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<ServiceOrder>(viewResult.Model);
    }

    [Fact]
    public async Task AddTask_AddsTaskToOrder()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.AddTask(order.Id, "Diagnostika", 800);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(_db.ServiceTasks);
        Assert.Equal("Diagnostika", _db.ServiceTasks.First().Name);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesOrder()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(order.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(_db.ServiceOrders);
    }
}
