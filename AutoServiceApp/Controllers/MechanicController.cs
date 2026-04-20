using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Controllers;

public class MechanicsController : Controller
{
    private readonly AppDbContext _db;

    public MechanicsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var mechanics = await _db.Mechanics
            .Include(x => x.ServiceOrderMechanics)            .OrderBy(x => x.LastName)
            .ToListAsync();

        return View(mechanics);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var mechanic = await _db.Mechanics
            .Include(x => x.ServiceOrderMechanics)
                .ThenInclude(x => x.ServiceOrder)
                    .ThenInclude(x => x!.Car)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (mechanic == null) return NotFound();

        return View(mechanic);
    }

    public IActionResult Create() => View(new Mechanic());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mechanic mechanic)
    {
        if (!ModelState.IsValid) return View(mechanic);

        _db.Mechanics.Add(mechanic);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var mechanic = await _db.Mechanics.FindAsync(id);
        if (mechanic == null) return NotFound();

        return View(mechanic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Mechanic mechanic)
    {
        if (id != mechanic.Id) return BadRequest();
        if (!ModelState.IsValid) return View(mechanic);

        _db.Mechanics.Update(mechanic);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var mechanic = await _db.Mechanics.FirstOrDefaultAsync(x => x.Id == id);
        if (mechanic == null) return NotFound();

        return View(mechanic);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var mechanic = await _db.Mechanics.FindAsync(id);
        if (mechanic == null) return NotFound();

        _db.Mechanics.Remove(mechanic);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}