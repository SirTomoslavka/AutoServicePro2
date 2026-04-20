using AutoServiceApp.Models;
using AutoServiceApp.Services;
using AutoServiceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class CarsController : Controller
{
    private readonly ICarService _cars;
    private readonly ICustomerService _customers;

    public CarsController(ICarService cars, ICustomerService customers)
    {
        _cars = cars;
        _customers = customers;
    }

    public async Task<IActionResult> Index() => View(await _cars.GetAllAsync());

    public async Task<IActionResult> Detail(Guid id)
    {
        var car = await _cars.GetByIdAsync(id);
        return car == null ? NotFound() : View(car);
    }

    public async Task<IActionResult> Create() =>
        View(new CarFormViewModel { Car = new Car(), Customers = await _customers.GetSelectListAsync() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Customers = await _customers.GetSelectListAsync();
            return View(vm);
        }
        await _cars.CreateAsync(vm.Car);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var car = await _cars.GetByIdAsync(id);
        if (car == null) return NotFound();
        return View(new CarFormViewModel { Car = car, Customers = await _customers.GetSelectListAsync() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CarFormViewModel vm)
    {
        if (id != vm.Car.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            vm.Customers = await _customers.GetSelectListAsync();
            return View(vm);
        }
        await _cars.UpdateAsync(vm.Car);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var car = await _cars.GetByIdAsync(id);
        return car == null ? NotFound() : View(car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _cars.DeleteAsync(id);
        if (result == null) return NotFound();
        if (!result.Value)
        {
            var car = await _cars.GetByIdAsync(id);
            ModelState.AddModelError(string.Empty, "Auto nelze smazat, protože má servisní zakázky.");
            return View(car);
        }
        return RedirectToAction(nameof(Index));
    }
}