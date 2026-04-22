using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class InvoiceServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _service = new InvoiceService(_db);
    }

    public void Dispose() => _db.Dispose();

    // ── Helpers ────────────────────────────────────────────────────────────

    private async Task<Invoice> CreateInvoiceWithTasksAndPartsAsync()
    {
        var customer = new Customer { FirstName = "Petra", LastName = "Nováčková" };
        var car = new Car { Brand = "BMW", Model = "320d", LicensePlate = "5EF4567", Customer = customer };
        var sparePart = new SparePart { Name = "Vstřikovač", UnitPrice = 800, StockQuantity = 10 };

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

        var order = new ServiceOrder
        {
            Car = car,
            Description = "Servis BMW",
            Tasks = new List<ServiceTask> { task1, task2 }
        };

        var invoice = new Invoice
        {
            InvoiceNumber = "FAK-TEST-001",
            ServiceOrder = order,
            TotalAmount = 999m, // intentionally stale snapshot
            IssuedAt = DateTime.UtcNow
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    // ── GetByIdAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedDto()
    {
        var invoice = await CreateInvoiceWithTasksAndPartsAsync();

        var dto = await _service.GetByIdAsync(invoice.Id);

        Assert.NotNull(dto);
        Assert.Equal("FAK-TEST-001", dto!.InvoiceNumber);
        Assert.Equal("Petra Nováčková", dto.CustomerName);
        Assert.Equal("BMW 320d (5EF4567)", dto.CarDisplay);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTasks_WithParts()
    {
        // This test would have caught the bug where GetByIdAsync
        // did not include ServiceTaskParts in the query.
        var invoice = await CreateInvoiceWithTasksAndPartsAsync();

        var dto = await _service.GetByIdAsync(invoice.Id);

        Assert.NotNull(dto);
        Assert.Equal(2, dto!.Tasks.Count);

        var taskWithPart = dto.Tasks.Single(t => t.Name == "Výměna vstřikovačů");
        Assert.Single(taskWithPart.Parts);
        Assert.Equal("Vstřikovač", taskWithPart.Parts[0].SparePartName);
        Assert.Equal(4, taskWithPart.Parts[0].Quantity);
        Assert.Equal(800m, taskWithPart.Parts[0].UnitPrice);
        Assert.Equal(3200m, taskWithPart.Parts[0].TotalPrice);
    }

    [Fact]
    public async Task GetByIdAsync_TotalAmount_IncludesPartsPrice()
    {
        // This test would have caught the original bug where TotalAmount
        // used the stale DB snapshot instead of live task+parts computation.
        var invoice = await CreateInvoiceWithTasksAndPartsAsync();

        var dto = await _service.GetByIdAsync(invoice.Id);

        Assert.NotNull(dto);
        // 1 200 (labour task 1) + 4×800 (parts) + 800 (labour task 2) = 5 200
        Assert.Equal(5200m, dto!.TotalAmount);
    }

    [Fact]
    public async Task GetByIdAsync_TotalAmount_NotStaleSnapshot()
    {
        // Verifies that the live-computed total overrides the stale TotalAmount
        // stored in the DB (999m in the helper above).
        var invoice = await CreateInvoiceWithTasksAndPartsAsync();

        var dto = await _service.GetByIdAsync(invoice.Id);

        // Stale value was 999m; correct value is 5 200m
        Assert.NotEqual(999m, dto!.TotalAmount);
        Assert.Equal(5200m, dto.TotalAmount);
    }

    [Fact]
    public async Task GetByIdAsync_TasksWithNoParts_TotalAmountEqualsTaskPrices()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder
        {
            Car = car,
            Description = "Bez dílů",
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Diagnostika", Price = 500 },
                new() { Name = "Seřízení", Price = 300 }
            }
        };
        var invoice = new Invoice { InvoiceNumber = "FAK-TEST-002", ServiceOrder = order, TotalAmount = 0, IssuedAt = DateTime.UtcNow };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var dto = await _service.GetByIdAsync(invoice.Id);

        Assert.Equal(800m, dto!.TotalAmount);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        var dto = await _service.GetByIdAsync(Guid.NewGuid());
        Assert.Null(dto);
    }

    // ── GetAllAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllInvoices()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order1 = new ServiceOrder { Car = car, Description = "A" };
        var order2 = new ServiceOrder { Car = car, Description = "B" };
        _db.Invoices.AddRange(
            new Invoice { InvoiceNumber = "FAK-001", ServiceOrder = order1, TotalAmount = 1000, IssuedAt = DateTime.UtcNow },
            new Invoice { InvoiceNumber = "FAK-002", ServiceOrder = order2, TotalAmount = 2000, IssuedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var list = await _service.GetAllAsync();

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsFlatCustomerName()
    {
        var customer = new Customer { FirstName = "Petra", LastName = "Nováčková" };
        var car = new Car { Brand = "BMW", Model = "320d", LicensePlate = "5EF4567", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        _db.Invoices.Add(new Invoice { InvoiceNumber = "FAK-001", ServiceOrder = order, TotalAmount = 5000, IssuedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();

        var list = await _service.GetAllAsync();

        Assert.Equal("Petra Nováčková", list[0].CustomerName);
        Assert.Equal("BMW 320d (5EF4567)", list[0].CarDisplay);
    }

    // ── MarkAsPaidAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task MarkAsPaidAsync_SetsIsPaidAndPaidAt()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        var invoice = new Invoice { InvoiceNumber = "FAK-001", ServiceOrder = order, TotalAmount = 1000, IssuedAt = DateTime.UtcNow };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var result = await _service.MarkAsPaidAsync(invoice.Id);

        Assert.True(result);
        var updated = await _db.Invoices.FindAsync(invoice.Id);
        Assert.True(updated!.IsPaid);
        Assert.NotNull(updated.PaidAt);
    }

    [Fact]
    public async Task MarkAsPaidAsync_InvalidId_ReturnsNull()
    {
        var result = await _service.MarkAsPaidAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    // ── DeleteAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_RemovesInvoice()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        var invoice = new Invoice { InvoiceNumber = "FAK-001", ServiceOrder = order, TotalAmount = 500, IssuedAt = DateTime.UtcNow };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        var result = await _service.DeleteAsync(invoice.Id);

        Assert.True(result);
        Assert.Empty(_db.Invoices);
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ReturnsNull()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());
        Assert.Null(result);
    }
}
