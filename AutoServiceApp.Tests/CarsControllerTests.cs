using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class CarsControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CarsController _controller;

    public CarsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        var carService = new CarService(_db);
        var customerService = new CustomerService(_db);
        _controller = new CarsController(carService, customerService);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Index_ReturnsViewWithCars()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        _db.Customers.Add(customer);
        _db.Cars.Add(new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", CustomerId = customer.Id });
        await _db.SaveChangesAsync();

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IList<CarDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Detail_WithValidId_ReturnsCarWithCustomer()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        var result = await _controller.Detail(car.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CarDto>(viewResult.Model);
        Assert.Equal("Škoda", model.Brand);
        Assert.NotNull(model.CustomerName);
    }

    [Fact]
    public async Task DeleteConfirmed_WithNoOrders_DeletesCar()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(car.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(_db.Cars);
    }

    [Fact]
    public async Task DeleteConfirmed_WithOrders_DoesNotDelete()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();

        _db.ServiceOrders.Add(new ServiceOrder { CarId = car.Id, Description = "Test order" });
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(car.Id);

        Assert.IsType<ViewResult>(result);
        Assert.Single(_db.Cars);
    }

    [Fact]
    public async Task Detail_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Detail(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }
}
