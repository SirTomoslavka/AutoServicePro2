using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Controllers;

public class CarsController : Controller
{
    private readonly AppDbContext _db;

    public CarsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var cars = await _db.Cars
            .Include(x => x.Customer)
            .OrderBy(x => x.Brand)
            .ThenBy(x => x.Model)
            .ToListAsync();

        return View(cars);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var car = await _db.Cars
            .Include(x => x.Customer)
            .Include(x => x.ServiceOrders)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (car == null) return NotFound();

        return View(car);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildFormVm(new Car()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CarFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Customers = await GetCustomersSelectList();
            return View(vm);
        }

        _db.Cars.Add(vm.Car);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var car = await _db.Cars.FindAsync(id);
        if (car == null) return NotFound();

        return View(await BuildFormVm(car));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CarFormViewModel vm)
    {
        if (id != vm.Car.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            vm.Customers = await GetCustomersSelectList();
            return View(vm);
        }

        _db.Cars.Update(vm.Car);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var car = await _db.Cars
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (car == null) return NotFound();

        return View(car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var car = await _db.Cars
            .Include(x => x.ServiceOrders)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (car == null) return NotFound();
        if (car.ServiceOrders.Any())
        {
            ModelState.AddModelError(string.Empty, "Auto nelze smazat, protože má servisní zakázky.");
            return View(car);
        }

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<CarFormViewModel> BuildFormVm(Car car)
    {
        return new CarFormViewModel
        {
            Car = car,
            Customers = await GetCustomersSelectList()
        };
    }

    private async Task<IEnumerable<SelectListItem>> GetCustomersSelectList()
    {
        return await _db.Customers
            .OrderBy(x => x.LastName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName
            })
            .ToListAsync();
    }
}