using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class SparePartsController : Controller
{
    private readonly ISparePartService _spareParts;

    public SparePartsController(ISparePartService spareParts) => _spareParts = spareParts;

    public async Task<IActionResult> Index() => View(await _spareParts.GetAllAsync());

    public async Task<IActionResult> Detail(Guid id)
    {
        var part = await _spareParts.GetByIdAsync(id);
        return part == null ? NotFound() : View(part);
    }

    public IActionResult Create() => View(new SparePart());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SparePart part)
    {
        if (!ModelState.IsValid) return View(part);
        await _spareParts.CreateAsync(part);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var part = await _spareParts.GetByIdAsync(id);
        return part == null ? NotFound() : View(part);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, SparePart part)
    {
        if (id != part.Id) return BadRequest();
        if (!ModelState.IsValid) return View(part);
        await _spareParts.UpdateAsync(part);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var part = await _spareParts.GetByIdAsync(id);
        return part == null ? NotFound() : View(part);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _spareParts.DeleteAsync(id);
        return result == null ? NotFound() : RedirectToAction(nameof(Index));
    }
}
