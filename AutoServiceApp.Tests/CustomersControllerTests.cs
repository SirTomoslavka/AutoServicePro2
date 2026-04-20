using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class CustomersControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        var service = new CustomerService(_db);
        _controller = new CustomersController(service);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Index_ReturnsViewWithCustomers()
    {
        _db.Customers.Add(new Customer { FirstName = "Jan", LastName = "Novák" });
        await _db.SaveChangesAsync();

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Detail_WithValidId_ReturnsView()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var result = await _controller.Detail(customer.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Customer>(viewResult.Model);
        Assert.Equal("Jan", model.FirstName);
    }

    [Fact]
    public async Task Detail_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Detail(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_Post_ValidCustomer_Redirects()
    {
        var customer = new Customer { FirstName = "Petr", LastName = "Dvořák", Email = "p@test.cz" };

        var result = await _controller.Create(customer);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Single(_db.Customers);
    }

    [Fact]
    public async Task DeleteConfirmed_WithNoCars_DeletesCustomer()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(customer.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(_db.Customers);
    }

    [Fact]
    public async Task DeleteConfirmed_WithCars_DoesNotDelete()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        _db.Cars.Add(new Car { Brand = "Škoda", Model = "Fabia", LicensePlate = "1AB1234", CustomerId = customer.Id });
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(customer.Id);

        Assert.IsType<ViewResult>(result);
        Assert.Single(_db.Customers);
    }

    [Fact]
    public async Task Edit_Post_ValidCustomer_Redirects()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        _db.Entry(customer).State = EntityState.Detached;

        var updated = new Customer { Id = customer.Id, FirstName = "Jana", LastName = "Nováková" };
        var result = await _controller.Edit(customer.Id, updated);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}
