using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class MechanicService : IMechanicService
{
    private readonly AppDbContext _db;

    public MechanicService(AppDbContext db) => _db = db;

    public async Task<IList<Mechanic>> GetAllAsync() =>
        await _db.Mechanics
            .Include(x => x.ServiceOrderMechanics)
            .OrderBy(x => x.LastName)
            .ToListAsync();

    public async Task<Mechanic?> GetByIdAsync(Guid id) =>
        await _db.Mechanics
            .Include(x => x.ServiceOrderMechanics)
                .ThenInclude(x => x.ServiceOrder)
                    .ThenInclude(x => x!.Car)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IList<SelectListItem>> GetSelectListAsync() =>
        await _db.Mechanics
            .OrderBy(x => x.LastName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName + " - " + x.Specialization
            })
            .ToListAsync();

    public async Task CreateAsync(Mechanic mechanic)
    {
        _db.Mechanics.Add(mechanic);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Mechanic mechanic)
    {
        _db.Mechanics.Update(mechanic);
        await _db.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(Guid id)
    {
        var mechanic = await _db.Mechanics.FindAsync(id);
        if (mechanic == null) return null;

        _db.Mechanics.Remove(mechanic);
        await _db.SaveChangesAsync();
        return true;
    }
}
