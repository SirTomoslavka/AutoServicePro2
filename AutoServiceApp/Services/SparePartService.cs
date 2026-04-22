using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class SparePartService : ISparePartService
{
    private readonly AppDbContext _db;

    public SparePartService(AppDbContext db) => _db = db;

    public async Task<IList<SparePartDto>> GetAllAsync() =>
        await _db.SpareParts
            .OrderBy(x => x.Name)
            .Select(x => new SparePartDto
            {
                Id = x.Id,
                Name = x.Name,
                CatalogNumber = x.CatalogNumber,
                UnitPrice = x.UnitPrice,
                StockQuantity = x.StockQuantity
            })
            .ToListAsync();

    public async Task<SparePartDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.SpareParts
            .Include(p => p.ServiceTaskParts)
                .ThenInclude(stp => stp.ServiceTask)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (x == null) return null;

        return new SparePartDto
        {
            Id = x.Id,
            Name = x.Name,
            CatalogNumber = x.CatalogNumber,
            UnitPrice = x.UnitPrice,
            StockQuantity = x.StockQuantity,
            Usages = x.ServiceTaskParts.Select(stp => new SparePartUsageDto
            {
                ServiceTaskName = stp.ServiceTask?.Name,
                Quantity = stp.Quantity,
                UnitPrice = stp.UnitPrice
            }).ToList()
        };
    }

    public async Task<SparePart?> GetByIdForEditAsync(Guid id) =>
        await _db.SpareParts.FindAsync(id);

    public async Task<IList<SelectListItem>> GetSelectListAsync() =>
        await _db.SpareParts
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.UnitPrice:N2} Kč)"
            })
            .ToListAsync();

    public async Task CreateAsync(SparePart sparePart)
    {
        _db.SpareParts.Add(sparePart);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(SparePart sparePart)
    {
        _db.SpareParts.Update(sparePart);
        await _db.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(Guid id)
    {
        var part = await _db.SpareParts.FindAsync(id);
        if (part == null) return null;

        _db.SpareParts.Remove(part);
        await _db.SaveChangesAsync();
        return true;
    }
}
