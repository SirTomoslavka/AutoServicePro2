using AutoServiceApp.Models;
using AutoServiceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _customers;

    public CustomersController(ICustomerService customers) => _customers = customers;

    public async Task<IActionResult> Index() => View(await _customers.GetAllAsync());

    public async Task<IActionResult> Detail(Guid id)
    {
        var customer = await _customers.GetByIdAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    public IActionResult Create() => View(new Customer());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid) return View(customer);
        await _customers.CreateAsync(customer);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var customer = await _customers.GetByIdForEditAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        if (!ModelState.IsValid) return View(customer);
        await _customers.UpdateAsync(customer);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _customers.GetByIdForEditAsync(id);
        return customer == null ? NotFound() : View(customer);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _customers.DeleteAsync(id);
        if (result == null) return NotFound();
        if (!result.Value)
        {
            var customer = await _customers.GetByIdForEditAsync(id);
            ModelState.AddModelError(string.Empty, "Zákazníka nelze smazat, protože má přiřazená auta.");
            return View(customer);
        }
        return RedirectToAction(nameof(Index));
    }
}