using AutoServiceApp.Data;
using AutoServiceApp.Models;
using AutoServiceApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Controllers;

public class ServiceOrdersController : Controller
{
    private readonly AppDbContext _db;

    public ServiceOrdersController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(ServiceOrderStatus? status)
    {
        var query = _db.ServiceOrders
            .Include(x => x.Car)
            .ThenInclude(x => x!.Customer)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        var serviceOrders = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        ViewBag.SelectedStatus = status;
        return View(serviceOrders);
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var serviceOrder = await _db.ServiceOrders
            .Include(x => x.Car)
            .ThenInclude(x => x!.Customer)
            .Include(x => x.Tasks)
            .Include(x => x.ServiceOrderMechanics)
            .ThenInclude(x => x.Mechanic)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (serviceOrder == null) return NotFound();

        return View(serviceOrder);
    }

    public async Task<IActionResult> Create()
    {
        return View(await BuildFormVm(new ServiceOrder()));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceOrderFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Cars = await GetCarsSelectList();
            return View(vm);
        }

        _db.ServiceOrders.Add(vm.ServiceOrder);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var serviceOrder = await _db.ServiceOrders.FindAsync(id);
        if (serviceOrder == null) return NotFound();

        return View(await BuildFormVm(serviceOrder));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ServiceOrderFormViewModel vm)
    {
        if (id != vm.ServiceOrder.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            vm.Cars = await GetCarsSelectList();
            return View(vm);
        }

        _db.ServiceOrders.Update(vm.ServiceOrder);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(Guid id)
    {
        var serviceOrder = await _db.ServiceOrders
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (serviceOrder == null) return NotFound();

        return View(serviceOrder);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var serviceOrder = await _db.ServiceOrders.FindAsync(id);
        if (serviceOrder == null) return NotFound();

        _db.ServiceOrders.Remove(serviceOrder);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTask(Guid serviceOrderId, string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RedirectToAction(nameof(Detail), new { id = serviceOrderId });
        }

        var task = new ServiceTask
        {
            ServiceOrderId = serviceOrderId,
            Name = name,
            Price = price
        };

        _db.ServiceTasks.Add(task);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Detail), new { id = serviceOrderId });
    }
    
    public async Task<IActionResult> AssignMechanics(Guid id)
    {
        var serviceOrder = await _db.ServiceOrders
            .Include(x => x.ServiceOrderMechanics)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (serviceOrder == null) return NotFound();

        var vm = new AssignMechanicsViewModel
        {
            ServiceOrderId = id,
            SelectedMechanicIds = serviceOrder.ServiceOrderMechanics.Select(x => x.MechanicId).ToList(),
            Mechanics = await _db.Mechanics
                .OrderBy(x => x.LastName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FirstName + " " + x.LastName + " - " + x.Specialization
                })
                .ToListAsync()
        };

        return View(vm);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignMechanics(AssignMechanicsViewModel vm)
    {
        var serviceOrder = await _db.ServiceOrders
            .Include(x => x.ServiceOrderMechanics)
            .FirstOrDefaultAsync(x => x.Id == vm.ServiceOrderId);

        if (serviceOrder == null) return NotFound();

        _db.ServiceOrderMechanics.RemoveRange(serviceOrder.ServiceOrderMechanics);

        var newLinks = vm.SelectedMechanicIds.Select(mechanicId => new ServiceOrderMechanic
        {
            ServiceOrderId = vm.ServiceOrderId,
            MechanicId = mechanicId
        });

        await _db.ServiceOrderMechanics.AddRangeAsync(newLinks);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Detail), new { id = vm.ServiceOrderId });
    }
    
    private async Task<ServiceOrderFormViewModel> BuildFormVm(ServiceOrder serviceOrder)
    {
        return new ServiceOrderFormViewModel
        {
            ServiceOrder = serviceOrder,
            Cars = await GetCarsSelectList()
        };
    }

    private async Task<IEnumerable<SelectListItem>> GetCarsSelectList()
    {
        return await _db.Cars
            .Include(x => x.Customer)
            .OrderBy(x => x.Brand)
            .ThenBy(x => x.Model)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Brand + " " + x.Model + " (" + x.LicensePlate + ") - " + x.Customer!.FirstName + " " + x.Customer.LastName
            })
            .ToListAsync();
    }
}