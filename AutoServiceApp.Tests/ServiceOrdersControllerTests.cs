using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
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
        var model = Assert.IsAssignableFrom<IList<ServiceOrderDto>>(viewResult.Model);
        Assert.Equal(2, model.Count);
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
        var model = Assert.IsAssignableFrom<IList<ServiceOrderDto>>(viewResult.Model);
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
        Assert.IsType<ServiceOrderDto>(viewResult.Model);
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

    [Fact]
    public async Task AddPartToTask_AddsPartToTask()
    {
        var sparePart = new SparePart { Name = "Olejový filtr", UnitPrice = 250, StockQuantity = 10 };
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var task = new ServiceTask { Name = "Výměna oleje", Price = 400 };
        var order = new ServiceOrder { Car = car, Description = "Test", Tasks = new List<ServiceTask> { task } };
        _db.SpareParts.Add(sparePart);
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.AddPartToTask(order.Id, task.Id, sparePart.Id, 2);

        Assert.IsType<RedirectToActionResult>(result);
        var stp = _db.ServiceTaskParts.Single();
        Assert.Equal(250m, stp.UnitPrice);
        Assert.Equal(2, stp.Quantity);
    }

    [Fact]
    public async Task AddPartToTask_InvalidSparePart_ReturnsNotFound()
    {
        var task = new ServiceTask { Name = "Test", Price = 100 };
        _db.ServiceTasks.Add(task);
        await _db.SaveChangesAsync();

        var result = await _controller.AddPartToTask(Guid.NewGuid(), task.Id, Guid.NewGuid(), 1);

        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(_db.ServiceTaskParts);
    }

    [Fact]
    public async Task RemovePartFromTask_RemovesPart()
    {
        var sparePart = new SparePart { Name = "Brzdové destičky", UnitPrice = 500, StockQuantity = 5 };
        var task = new ServiceTask { Name = "Výměna brzd", Price = 800 };
        var stp = new ServiceTaskPart { ServiceTask = task, SparePart = sparePart, Quantity = 1, UnitPrice = 500 };
        _db.ServiceTaskParts.Add(stp);
        await _db.SaveChangesAsync();

        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.RemovePartFromTask(order.Id, stp.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(_db.ServiceTaskParts);
    }

    [Fact]
    public async Task GenerateInvoice_CreatesInvoiceWithPartsInTotal()
    {
        // This test would have caught the original bug where parts were not
        // included in ServiceTaskParts when computing TotalAmount.
        var sparePart = new SparePart { Name = "Vstřikovač", UnitPrice = 800, StockQuantity = 10 };
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "BMW", Model = "320d", LicensePlate = "5EF4567", Customer = customer };
        var task = new ServiceTask
        {
            Name = "Výměna vstřikovačů",
            Price = 1200,
            ServiceTaskParts = new List<ServiceTaskPart>
            {
                new() { SparePart = sparePart, Quantity = 4, UnitPrice = 800 }  // 3 200
            }
        };
        var order = new ServiceOrder { Car = car, Description = "Test", Tasks = new List<ServiceTask> { task } };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.GenerateInvoice(order.Id);

        Assert.IsType<RedirectToActionResult>(result);
        var invoice = _db.Invoices.Single();
        // 1 200 (labour) + 4×800 (parts) = 4 400
        Assert.Equal(4400m, invoice.TotalAmount);
    }

    [Fact]
    public async Task Detail_IncludesTasksAndPartsInDto()
    {
        var sparePart = new SparePart { Name = "Olejový filtr", UnitPrice = 250, StockQuantity = 10 };
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var task = new ServiceTask
        {
            Name = "Výměna oleje",
            Price = 400,
            ServiceTaskParts = new List<ServiceTaskPart>
            {
                new() { SparePart = sparePart, Quantity = 1, UnitPrice = 250 }
            }
        };
        var order = new ServiceOrder { Car = car, Description = "Test", Tasks = new List<ServiceTask> { task } };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _controller.Detail(order.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var dto = Assert.IsType<ServiceOrderDto>(viewResult.Model);
        Assert.Single(dto.Tasks);
        Assert.Single(dto.Tasks[0].Parts);
        Assert.Equal("Olejový filtr", dto.Tasks[0].Parts[0].SparePartName);
        Assert.Equal(650m, dto.TotalPrice); // 400 + 250
    }
}

