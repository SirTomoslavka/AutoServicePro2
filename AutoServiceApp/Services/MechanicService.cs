using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class MechanicService : IMechanicService
{
    private readonly AppDbContext _db;

    public MechanicService(AppDbContext db) => _db = db;

    public async Task<IList<MechanicDto>> GetAllAsync() =>
        await _db.Mechanics
            .Include(x => x.ServiceOrderMechanics)
            .OrderBy(x => x.LastName)
            .Select(x => new MechanicDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Specialization = x.Specialization,
                OrderCount = x.ServiceOrderMechanics.Count
            })
            .ToListAsync();

    public async Task<MechanicDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.Mechanics
            .Include(x => x.ServiceOrderMechanics)
                .ThenInclude(x => x.ServiceOrder)
                    .ThenInclude(x => x!.Car)
                        .ThenInclude(x => x!.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (x == null) return null;

        return new MechanicDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Specialization = x.Specialization,
            OrderCount = x.ServiceOrderMechanics.Count,
            Orders = x.ServiceOrderMechanics
                .Where(m => m.ServiceOrder != null)
                .Select(m => new ServiceOrderSummaryDto
                {
                    Id = m.ServiceOrder!.Id,
                    CreatedAt = m.ServiceOrder.CreatedAt,
                    Status = m.ServiceOrder.Status,
                    Description = m.ServiceOrder.Description,
                    CarDisplay = m.ServiceOrder.Car != null
                        ? $"{m.ServiceOrder.Car.Brand} {m.ServiceOrder.Car.Model} ({m.ServiceOrder.Car.LicensePlate})"
                        : null,
                    CustomerName = m.ServiceOrder.Car?.Customer != null
                        ? $"{m.ServiceOrder.Car.Customer.FirstName} {m.ServiceOrder.Car.Customer.LastName}"
                        : null
                })
                .ToList()
        };
    }

    public async Task<Mechanic?> GetByIdForEditAsync(Guid id) =>
        await _db.Mechanics.FindAsync(id);

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
