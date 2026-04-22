using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class ServiceOrderServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly ServiceOrderService _service;

    public ServiceOrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _service = new ServiceOrderService(_db);
    }

    public void Dispose() => _db.Dispose();

    // ── Helpers ────────────────────────────────────────────────────────────

    private async Task<ServiceOrder> CreateOrderWithTaskAndPartsAsync()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "BMW", Model = "320d", LicensePlate = "5EF4567", Customer = customer };
        var sparePart = new SparePart { Name = "Vstřikovač", UnitPrice = 800, StockQuantity = 10 };

        var task = new ServiceTask
        {
            Name = "Výměna vstřikovačů",
            Price = 1200,
            ServiceTaskParts = new List<ServiceTaskPart>
            {
                new() { SparePart = sparePart, Quantity = 4, UnitPrice = 800 }  // 4 × 800 = 3 200
            }
        };

        var order = new ServiceOrder
        {
            Car = car,
            Description = "Test zakázka",
            Status = ServiceOrderStatus.InProgress,
            Tasks = new List<ServiceTask> { task }
        };

        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }

    // ── GetByIdAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedDto()
    {
        var order = await CreateOrderWithTaskAndPartsAsync();

        var dto = await _service.GetByIdAsync(order.Id);

        Assert.NotNull(dto);
        Assert.Equal(order.Id, dto!.Id);
        Assert.Equal("BMW", dto.CarBrand);
        Assert.Equal("Jan Novák", dto.CustomerName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTasks_WithParts()
    {
        var order = await CreateOrderWithTaskAndPartsAsync();

        var dto = await _service.GetByIdAsync(order.Id);

        Assert.NotNull(dto);
        Assert.Single(dto!.Tasks);
        var task = dto.Tasks[0];
        Assert.Single(task.Parts);
        Assert.Equal("Vstřikovač", task.Parts[0].SparePartName);
        Assert.Equal(4, task.Parts[0].Quantity);
        Assert.Equal(800m, task.Parts[0].UnitPrice);
    }

    [Fact]
    public async Task GetByIdAsync_TotalPrice_IncludesPartsPrice()
    {
        // This test would have caught the original bug:
        // TotalPrice was only summing task.Price, ignoring parts.
        var order = await CreateOrderWithTaskAndPartsAsync();

        var dto = await _service.GetByIdAsync(order.Id);

        Assert.NotNull(dto);
        // 1 200 (labour) + 4 × 800 (parts) = 4 400
        Assert.Equal(4400m, dto!.TotalPrice);
    }

    [Fact]
    public async Task GetByIdAsync_TotalPrice_NoPartsEqualsTaskPriceOnly()
    {
        var customer = new Customer { FirstName = "Petr", LastName = "Malý" };
        var car = new Car { Brand = "Škoda", Model = "Fabia", LicensePlate = "1XX0001", Customer = customer };
        var order = new ServiceOrder
        {
            Car = car,
            Description = "Bez dílů",
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Diagnostika", Price = 500 }
            }
        };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var dto = await _service.GetByIdAsync(order.Id);

        Assert.Equal(500m, dto!.TotalPrice);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsNull()
    {
        var dto = await _service.GetByIdAsync(Guid.NewGuid());
        Assert.Null(dto);
    }

    // ── GetAllAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "A", Status = ServiceOrderStatus.New });
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "B", Status = ServiceOrderStatus.Done });
        await _db.SaveChangesAsync();

        var list = await _service.GetAllAsync();

        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task GetAllAsync_FiltersByStatus()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "A", Status = ServiceOrderStatus.New });
        _db.ServiceOrders.Add(new ServiceOrder { Car = car, Description = "B", Status = ServiceOrderStatus.Done });
        await _db.SaveChangesAsync();

        var list = await _service.GetAllAsync(ServiceOrderStatus.New);

        Assert.Single(list);
        Assert.Equal(ServiceOrderStatus.New, list[0].Status);
    }

    // ── GenerateInvoiceAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GenerateInvoiceAsync_TotalAmount_IncludesPartsPrice()
    {
        // This test would have caught the original invoice bug:
        // TotalAmount was computed without loading ServiceTaskParts.
        var order = await CreateOrderWithTaskAndPartsAsync();

        var invoice = await _service.GenerateInvoiceAsync(order.Id);

        Assert.NotNull(invoice);
        // 1 200 (labour) + 4 × 800 (parts) = 4 400
        Assert.Equal(4400m, invoice!.TotalAmount);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_TotalAmount_TasksOnly_NoPartsPrice()
    {
        var customer = new Customer { FirstName = "Petr", LastName = "Malý" };
        var car = new Car { Brand = "Škoda", Model = "Fabia", LicensePlate = "1XX0001", Customer = customer };
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
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var invoice = await _service.GenerateInvoiceAsync(order.Id);

        Assert.Equal(800m, invoice!.TotalAmount);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_CreatesInvoiceRecord()
    {
        var order = await CreateOrderWithTaskAndPartsAsync();

        await _service.GenerateInvoiceAsync(order.Id);

        Assert.Single(_db.Invoices);
        Assert.False(_db.Invoices.First().IsPaid);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_CalledTwice_DoesNotDuplicate()
    {
        var order = await CreateOrderWithTaskAndPartsAsync();

        await _service.GenerateInvoiceAsync(order.Id);
        await _service.GenerateInvoiceAsync(order.Id);

        Assert.Single(_db.Invoices);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_InvalidOrderId_ReturnsNull()
    {
        var result = await _service.GenerateInvoiceAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    // ── AddTaskAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task AddTaskAsync_AddsTaskWithCorrectPrice()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234", Customer = customer };
        var order = new ServiceOrder { Car = car, Description = "Test" };
        _db.ServiceOrders.Add(order);
        await _db.SaveChangesAsync();

        var result = await _service.AddTaskAsync(order.Id, "Diagnostika", 800);

        Assert.True(result);
        var task = _db.ServiceTasks.Single();
        Assert.Equal("Diagnostika", task.Name);
        Assert.Equal(800m, task.Price);
        Assert.Equal(order.Id, task.ServiceOrderId);
    }

    [Fact]
    public async Task AddTaskAsync_InvalidOrderId_ReturnsFalse()
    {
        var result = await _service.AddTaskAsync(Guid.NewGuid(), "Test", 100);
        Assert.False(result);
    }

    // ── AddPartToTaskAsync ──────────────────────────────────────────────────

    [Fact]
    public async Task AddPartToTaskAsync_AddsPartWithSparePartUnitPrice()
    {
        var sparePart = new SparePart { Name = "Olejový filtr", UnitPrice = 250, StockQuantity = 20 };
        var task = new ServiceTask { Name = "Výměna oleje", Price = 400 };
        _db.SpareParts.Add(sparePart);
        _db.ServiceTasks.Add(task);
        await _db.SaveChangesAsync();

        var result = await _service.AddPartToTaskAsync(task.Id, sparePart.Id, 2);

        Assert.True(result);
        var stp = _db.ServiceTaskParts.Single();
        Assert.Equal(250m, stp.UnitPrice);   // must copy price from SparePart
        Assert.Equal(2, stp.Quantity);
        Assert.Equal(task.Id, stp.ServiceTaskId);
    }

    [Fact]
    public async Task AddPartToTaskAsync_SamePartTwice_IncrementsQuantity()
    {
        var sparePart = new SparePart { Name = "Šroub", UnitPrice = 10, StockQuantity = 100 };
        var task = new ServiceTask { Name = "Montáž", Price = 200 };
        _db.SpareParts.Add(sparePart);
        _db.ServiceTasks.Add(task);
        await _db.SaveChangesAsync();

        await _service.AddPartToTaskAsync(task.Id, sparePart.Id, 3);
        await _service.AddPartToTaskAsync(task.Id, sparePart.Id, 2);

        var stp = _db.ServiceTaskParts.Single(); // still one row
        Assert.Equal(5, stp.Quantity);           // 3 + 2
    }

    [Fact]
    public async Task AddPartToTaskAsync_NonexistentSparePart_ReturnsNull()
    {
        var task = new ServiceTask { Name = "Test", Price = 100 };
        _db.ServiceTasks.Add(task);
        await _db.SaveChangesAsync();

        var result = await _service.AddPartToTaskAsync(task.Id, Guid.NewGuid(), 1);

        Assert.Null(result);
        Assert.Empty(_db.ServiceTaskParts);
    }

    // ── RemovePartFromTaskAsync ─────────────────────────────────────────────

    [Fact]
    public async Task RemovePartFromTaskAsync_RemovesPart()
    {
        var sparePart = new SparePart { Name = "Brzdový kotouč", UnitPrice = 600, StockQuantity = 5 };
        var task = new ServiceTask { Name = "Výměna brzd", Price = 800 };
        var stp = new ServiceTaskPart { ServiceTask = task, SparePart = sparePart, Quantity = 2, UnitPrice = 600 };
        _db.ServiceTaskParts.Add(stp);
        await _db.SaveChangesAsync();

        await _service.RemovePartFromTaskAsync(stp.Id);

        Assert.Empty(_db.ServiceTaskParts);
    }

    [Fact]
    public async Task RemovePartFromTaskAsync_InvalidId_DoesNotThrow()
    {
        var ex = await Record.ExceptionAsync(() => _service.RemovePartFromTaskAsync(Guid.NewGuid()));
        Assert.Null(ex);
    }
}
