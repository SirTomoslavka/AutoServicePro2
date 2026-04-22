using AutoServiceApp.Controllers;
using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Tests;

public class SparePartsControllerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly SparePartsController _controller;

    public SparePartsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        var service = new SparePartService(_db);
        _controller = new SparePartsController(service);
    }

    public void Dispose() => _db.Dispose();

    [Fact]
    public async Task Index_ReturnsViewWithParts()
    {
        _db.SpareParts.Add(new SparePart { Name = "Brzdové destičky", UnitPrice = 500, StockQuantity = 10 });
        await _db.SaveChangesAsync();

        var result = await _controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IList<SparePartDto>>(viewResult.Model);
        Assert.Single(model);
    }

    [Fact]
    public async Task Create_Post_ValidPart_Redirects()
    {
        var part = new SparePart { Name = "Olejový filtr", UnitPrice = 250, StockQuantity = 20 };

        var result = await _controller.Create(part);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Single(_db.SpareParts);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesPart()
    {
        var part = new SparePart { Name = "Olejový filtr", UnitPrice = 250, StockQuantity = 20 };
        _db.SpareParts.Add(part);
        await _db.SaveChangesAsync();

        var result = await _controller.DeleteConfirmed(part.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Empty(_db.SpareParts);
    }

    [Fact]
    public async Task Detail_WithInvalidId_ReturnsNotFound()
    {
        var result = await _controller.Detail(Guid.NewGuid());
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ValidPart_Redirects()
    {
        var part = new SparePart { Name = "Filtr", UnitPrice = 200, StockQuantity = 5 };
        _db.SpareParts.Add(part);
        await _db.SaveChangesAsync();

        _db.Entry(part).State = EntityState.Detached;

        var updated = new SparePart { Id = part.Id, Name = "Vzduchový filtr", UnitPrice = 300, StockQuantity = 10 };
        var result = await _controller.Edit(part.Id, updated);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}
