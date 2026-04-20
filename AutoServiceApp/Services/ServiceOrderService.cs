using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class ServiceOrderService : IServiceOrderService
{
    private readonly AppDbContext _db;

    public ServiceOrderService(AppDbContext db) => _db = db;

    public async Task<IList<ServiceOrder>> GetAllAsync(ServiceOrderStatus? status = null)
    {
        var query = _db.ServiceOrders
            .Include(x => x.Car)
                .ThenInclude(x => x!.Customer)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public async Task<ServiceOrder?> GetByIdAsync(Guid id) =>
        await _db.ServiceOrders
            .Include(x => x.Car).ThenInclude(x => x!.Customer)
            .Include(x => x.Tasks)
                .ThenInclude(x => x.ServiceTaskParts)
                    .ThenInclude(x => x.SparePart)
            .Include(x => x.ServiceOrderMechanics).ThenInclude(x => x.Mechanic)
            .Include(x => x.Invoice)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ServiceOrder?> GetByIdForEditAsync(Guid id) =>
        await _db.ServiceOrders.FindAsync(id);

    public async Task<ServiceOrder?> GetByIdForDeleteAsync(Guid id) =>
        await _db.ServiceOrders
            .Include(x => x.Car)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ServiceOrder?> GetByIdWithMechanicsAsync(Guid id) =>
        await _db.ServiceOrders
            .Include(x => x.ServiceOrderMechanics)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task CreateAsync(ServiceOrder serviceOrder)
    {
        _db.ServiceOrders.Add(serviceOrder);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(ServiceOrder serviceOrder)
    {
        _db.ServiceOrders.Update(serviceOrder);
        await _db.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(Guid id)
    {
        var order = await _db.ServiceOrders.FindAsync(id);
        if (order == null) return null;

        _db.ServiceOrders.Remove(order);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddTaskAsync(Guid serviceOrderId, string name, decimal price)
    {
        var exists = await _db.ServiceOrders.AnyAsync(x => x.Id == serviceOrderId);
        if (!exists) return false;

        _db.ServiceTasks.Add(new ServiceTask
        {
            ServiceOrderId = serviceOrderId,
            Name = name,
            Price = price
        });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool?> AssignMechanicsAsync(Guid serviceOrderId, IList<Guid> mechanicIds)
    {
        var serviceOrder = await _db.ServiceOrders
            .Include(x => x.ServiceOrderMechanics)
            .FirstOrDefaultAsync(x => x.Id == serviceOrderId);

        if (serviceOrder == null) return null;

        _db.ServiceOrderMechanics.RemoveRange(serviceOrder.ServiceOrderMechanics);
        await _db.ServiceOrderMechanics.AddRangeAsync(mechanicIds.Select(mechanicId => new ServiceOrderMechanic
        {
            ServiceOrderId = serviceOrderId,
            MechanicId = mechanicId
        }));

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool?> ChangeStatusAsync(Guid id, ServiceOrderStatus newStatus)
    {
        var order = await _db.ServiceOrders.FindAsync(id);
        if (order == null) return null;

        order.Status = newStatus;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Invoice?> GenerateInvoiceAsync(Guid serviceOrderId)
    {
        var serviceOrder = await _db.ServiceOrders
            .Include(x => x.Tasks)
            .Include(x => x.Invoice)
            .FirstOrDefaultAsync(x => x.Id == serviceOrderId);

        if (serviceOrder == null) return null;
        if (serviceOrder.Invoice != null) return serviceOrder.Invoice;

        var invoice = new Invoice
        {
            InvoiceNumber = $"FAK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            ServiceOrderId = serviceOrderId,
            TotalAmount = serviceOrder.TotalPrice,
            IssuedAt = DateTime.UtcNow
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();
        return invoice;
    }

    public async Task<bool?> AddPartToTaskAsync(Guid serviceTaskId, Guid sparePartId, int quantity)
    {
        var sparePart = await _db.SpareParts.FindAsync(sparePartId);
        if (sparePart == null) return null;

        var existing = await _db.ServiceTaskParts
            .FirstOrDefaultAsync(x => x.ServiceTaskId == serviceTaskId && x.SparePartId == sparePartId);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _db.ServiceTaskParts.Add(new ServiceTaskPart
            {
                ServiceTaskId = serviceTaskId,
                SparePartId = sparePartId,
                Quantity = quantity,
                UnitPrice = sparePart.UnitPrice
            });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task RemovePartFromTaskAsync(Guid serviceTaskPartId)
    {
        var stp = await _db.ServiceTaskParts.FindAsync(serviceTaskPartId);
        if (stp != null)
        {
            _db.ServiceTaskParts.Remove(stp);
            await _db.SaveChangesAsync();
        }
    }
}
