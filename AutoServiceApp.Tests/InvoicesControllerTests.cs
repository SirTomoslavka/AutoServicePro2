using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class InvoicesControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly InvoicesController _controller;

    public InvoicesControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        var service = new InvoiceService(_db);
        _controller = new InvoicesController(service);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Index_ReturnsViewWithInvoices()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        _db.Invoices.Add(new Invoice { InvoiceNumber = "FAK-001", TotalAmount = 5000, ServiceOrder = order });
        await _db.SaveChangesAsync();

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Invoice>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task MarkAsPaid_SetsIsPaidTrue()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        var invoice = new Invoice { InvoiceNumber = "FAK-001", TotalAmount = 5000, ServiceOrder = order };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var result = await _controller.MarkAsPaid(invoice.Id);

        Assert.IsType<RedirectToActionResult>(result);
        var updated = await _db.Invoices.FindAsync(invoice.Id);
        Assert.True(updated!.IsPaid);
        Assert.NotNull(updated.PaidAt);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesInvoice()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        var invoice = new Invoice { InvoiceNumber = "FAK-002", TotalAmount = 3000, ServiceOrder = order };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(invoice.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(_db.Invoices);
    }

    [Fact]
    public async Task Detail_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Detail(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }
}
