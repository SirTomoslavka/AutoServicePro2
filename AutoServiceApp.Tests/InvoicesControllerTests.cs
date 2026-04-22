using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

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
        var pdfMock = new Mock<IInvoicePdfService>();
        _controller = new InvoicesController(service, pdfMock.Object);
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
        var model = Assert.IsAssignableFrom<IList<InvoiceDto>>(viewResult.Model);
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

    [Fact]
    public async Task Detail_ReturnsTasks_WithParts()
    {
        // This test would have caught the bug where Detail did not load
        // ServiceTaskParts, so parts were invisible in the invoice detail view.
        var sparePart = new SparePart { Name = "Vstřikovač", UnitPrice = 800, StockQuantity = 10 };
        var customer = new Customer { FirstName = "Petra", LastName = "Nováčková" };
        var car = new Car { Brand = "BMW", Model = "320d", LicensePlate = "5EF4567", Customer = customer };
        var task = new ServiceTask
        {
            Name = "Výměna vstřikovačů",
            Price = 1200,
            ServiceTaskParts = new List<ServiceTaskPart>
            {
                new() { SparePart = sparePart, Quantity = 4, UnitPrice = 800 }
            }
        };
        var order = new ServiceOrder { Car = car, Description = "Test", Tasks = new List<ServiceTask> { task } };
        var invoice = new Invoice { InvoiceNumber = "FAK-TEST", ServiceOrder = order, TotalAmount = 0, IssuedAt = DateTime.UtcNow };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var result = await _controller.Detail(invoice.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var dto = Assert.IsType<InvoiceDto>(viewResult.Model);
        Assert.Single(dto.Tasks);
        Assert.Single(dto.Tasks[0].Parts);
        Assert.Equal("Vstřikovač", dto.Tasks[0].Parts[0].SparePartName);
        Assert.Equal(3200m, dto.Tasks[0].Parts[0].TotalPrice);
    }

    [Fact]
    public async Task Detail_TotalAmount_IncludesPartsPrice()
    {
        // This test would have caught the bug where TotalAmount used the stale
        // DB snapshot instead of being computed live from tasks + parts.
        var sparePart = new SparePart { Name = "Vstřikovač", UnitPrice = 800, StockQuantity = 10 };
        var customer = new Customer { FirstName = "Petra", LastName = "Nováčková" };
        var car = new Car { Brand = "BMW", Model = "320d", LicensePlate = "5EF4567", Customer = customer };
        var task1 = new ServiceTask
        {
            Name = "Výměna vstřikovačů",
            Price = 1200,
            ServiceTaskParts = new List<ServiceTaskPart>
            {
                new() { SparePart = sparePart, Quantity = 4, UnitPrice = 800 }  // 3 200
            }
        };
        var task2 = new ServiceTask { Name = "Diagnostika motoru", Price = 800 };
        var order = new ServiceOrder { Car = car, Description = "Test", Tasks = new List<ServiceTask> { task1, task2 } };
        var invoice = new Invoice { InvoiceNumber = "FAK-TEST", ServiceOrder = order, TotalAmount = 999m, IssuedAt = DateTime.UtcNow };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var result = await _controller.Detail(invoice.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        var dto = Assert.IsType<InvoiceDto>(viewResult.Model);
        // 1 200 + 3 200 + 800 = 5 200  (not the stale 999)
        Assert.Equal(5200m, dto.TotalAmount);
        Assert.NotEqual(999m, dto.TotalAmount);
    }
}
