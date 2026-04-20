using AutoServiceApp.Models;
using AutoServiceApp.Services;
using AutoServiceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class ServiceOrdersController : Controller
{
    private readonly IServiceOrderService _serviceOrders;
    private readonly ICarService _cars;
    private readonly IMechanicService _mechanics;
    private readonly ISparePartService _spareParts;

    public ServiceOrdersController(
        IServiceOrderService serviceOrders,
        ICarService cars,
        IMechanicService mechanics,
        ISparePartService spareParts)
    {
        _serviceOrders = serviceOrders;
        _cars = cars;
        _mechanics = mechanics;
        _spareParts = spareParts;
    }

    public async Task<IActionResult> Index(ServiceOrderStatus? status)
    {
        ViewBag.SelectedStatus = status;
        return View(await _serviceOrders.GetAllAsync(status));
    }

    public async Task<IActionResult> Detail(Guid id)
    {
        var serviceOrder = await _serviceOrders.GetByIdAsync(id);
        if (serviceOrder == null) return NotFound();

        ViewBag.SpareParts = await _spareParts.GetSelectListAsync();
        return View(serviceOrder);
    }

    public async Task<IActionResult> Create() =>
        View(new ServiceOrderFormViewModel { ServiceOrder = new ServiceOrder(), Cars = await _cars.GetSelectListAsync() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceOrderFormViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Cars = await _cars.GetSelectListAsync();
            return View(vm);
        }
        await _serviceOrders.CreateAsync(vm.ServiceOrder);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var serviceOrder = await _serviceOrders.GetByIdForEditAsync(id);
        if (serviceOrder == null) return NotFound();
        return View(new ServiceOrderFormViewModel { ServiceOrder = serviceOrder, Cars = await _cars.GetSelectListAsync() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ServiceOrderFormViewModel vm)
    {
        if (id != vm.ServiceOrder.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            vm.Cars = await _cars.GetSelectListAsync();
            return View(vm);
        }
        await _serviceOrders.UpdateAsync(vm.ServiceOrder);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var serviceOrder = await _serviceOrders.GetByIdForDeleteAsync(id);
        return serviceOrder == null ? NotFound() : View(serviceOrder);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _serviceOrders.DeleteAsync(id);
        return result == null ? NotFound() : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTask(Guid serviceOrderId, string name, decimal price)
    {
        if (!string.IsNullOrWhiteSpace(name))
            await _serviceOrders.AddTaskAsync(serviceOrderId, name, price);

        return RedirectToAction(nameof(Detail), new { id = serviceOrderId });
    }

    public async Task<IActionResult> AssignMechanics(Guid id)
    {
        var serviceOrder = await _serviceOrders.GetByIdWithMechanicsAsync(id);
        if (serviceOrder == null) return NotFound();

        var vm = new AssignMechanicsViewModel
        {
            ServiceOrderId = id,
            SelectedMechanicIds = serviceOrder.ServiceOrderMechanics.Select(x => x.MechanicId).ToList(),
            Mechanics = await _mechanics.GetSelectListAsync()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignMechanics(AssignMechanicsViewModel vm)
    {
        var result = await _serviceOrders.AssignMechanicsAsync(vm.ServiceOrderId, vm.SelectedMechanicIds);
        return result == null ? NotFound() : RedirectToAction(nameof(Detail), new { id = vm.ServiceOrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(Guid id, ServiceOrderStatus newStatus)
    {
        var result = await _serviceOrders.ChangeStatusAsync(id, newStatus);
        return result == null ? NotFound() : RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateInvoice(Guid serviceOrderId)
    {
        var invoice = await _serviceOrders.GenerateInvoiceAsync(serviceOrderId);
        return invoice == null ? NotFound() : RedirectToAction("Detail", "Invoices", new { id = invoice.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPartToTask(Guid serviceOrderId, Guid serviceTaskId, Guid sparePartId, int quantity)
    {
        if (quantity <= 0) quantity = 1;
        var result = await _serviceOrders.AddPartToTaskAsync(serviceTaskId, sparePartId, quantity);
        return result == null ? NotFound() : RedirectToAction(nameof(Detail), new { id = serviceOrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePartFromTask(Guid serviceOrderId, Guid serviceTaskPartId)
    {
        await _serviceOrders.RemovePartFromTaskAsync(serviceTaskPartId);
        return RedirectToAction(nameof(Detail), new { id = serviceOrderId });
    }
}