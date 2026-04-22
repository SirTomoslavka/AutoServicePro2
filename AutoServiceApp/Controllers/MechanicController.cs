using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class MechanicsController : Controller
{
    private readonly IMechanicService _mechanics;

    public MechanicsController(IMechanicService mechanics) => _mechanics = mechanics;

    public async Task<IActionResult> Index() => View(await _mechanics.GetAllAsync());

    public async Task<IActionResult> Detail(Guid id)
    {
        var mechanic = await _mechanics.GetByIdAsync(id);
        return mechanic == null ? NotFound() : View(mechanic);
    }

    public IActionResult Create() => View(new Mechanic());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Mechanic mechanic)
    {
        if (!ModelState.IsValid) return View(mechanic);
        await _mechanics.CreateAsync(mechanic);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var mechanic = await _mechanics.GetByIdForEditAsync(id);
        return mechanic == null ? NotFound() : View(mechanic);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Mechanic mechanic)
    {
        if (id != mechanic.Id) return BadRequest();
        if (!ModelState.IsValid) return View(mechanic);
        await _mechanics.UpdateAsync(mechanic);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var mechanic = await _mechanics.GetByIdForEditAsync(id);
        return mechanic == null ? NotFound() : View(mechanic);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _mechanics.DeleteAsync(id);
        return result == null ? NotFound() : RedirectToAction(nameof(Index));
    }
}