using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class SparePartService : ISparePartService
{
    private readonly AppDbContext _db;

    public SparePartService(AppDbContext db) => _db = db;

    public async Task<IList<SparePart>> GetAllAsync() =>
        await _db.SpareParts.OrderBy(x => x.Name).ToListAsync();

    public async Task<SparePart?> GetByIdAsync(Guid id) =>
        await _db.SpareParts
            .Include(x => x.ServiceTaskParts)
                .ThenInclude(x => x.ServiceTask)
            .FirstOrDefaultAsync(x => x.Id == id);

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
