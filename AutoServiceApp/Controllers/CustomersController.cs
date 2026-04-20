using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Controllers;

public class CustomersController : Controller
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _db.Customers
            .Include(x => x.Cars)
            .OrderBy(x => x.LastName)
            .ToListAsync();
        return View(customers);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var customer = await _db.Customers
            .Include(x => x.Cars)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (customer == null) return NotFound();

        return View(customer);
    }

    public IActionResult Create()
    {
        return View(new Customer());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null) return NotFound();

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        if (!ModelState.IsValid) return View(customer);

        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _db.Customers.FirstOrDefaultAsync(x => x.Id == id);
        if (customer == null) return NotFound();

        return View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var customer = await _db.Customers
            .Include(x => x.Cars)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (customer == null) return NotFound();
        if (customer.Cars.Any())
        {
            ModelState.AddModelError(string.Empty, "Zákazníka nelze smazat, protože má přiřazená auta.");
            return View(customer);
        }

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}