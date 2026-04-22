using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;

    public InvoiceService(AppDbContext db) => _db = db;

    public async Task<IList<InvoiceDto>> GetAllAsync()
    {
        var invoices = await _db.Invoices
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x!.Car)
                    .ThenInclude(x => x!.Customer)
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync();

        return invoices.Select(x => new InvoiceDto
        {
            Id = x.Id,
            InvoiceNumber = x.InvoiceNumber,
            IssuedAt = x.IssuedAt,
            PaidAt = x.PaidAt,
            IsPaid = x.IsPaid,
            TotalAmount = x.TotalAmount,
            Note = x.Note,
            ServiceOrderId = x.ServiceOrderId,
            CustomerName = x.ServiceOrder?.Car?.Customer != null
                ? $"{x.ServiceOrder.Car.Customer.FirstName} {x.ServiceOrder.Car.Customer.LastName}"
                : null,
            CarDisplay = x.ServiceOrder?.Car != null
                ? $"{x.ServiceOrder.Car.Brand} {x.ServiceOrder.Car.Model} ({x.ServiceOrder.Car.LicensePlate})"
                : null
        }).ToList();
    }

    public async Task<InvoiceDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.Invoices
            .Include(i => i.ServiceOrder)
                .ThenInclude(o => o!.Car)
                    .ThenInclude(c => c!.Customer)
            .Include(i => i.ServiceOrder)
                .ThenInclude(o => o!.Tasks)
                    .ThenInclude(t => t.ServiceTaskParts)
                        .ThenInclude(p => p.SparePart)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (x == null) return null;

        return new InvoiceDto
        {
            Id = x.Id,
            InvoiceNumber = x.InvoiceNumber,
            IssuedAt = x.IssuedAt,
            PaidAt = x.PaidAt,
            IsPaid = x.IsPaid,
            TotalAmount = x.ServiceOrder?.Tasks.Sum(t =>
                t.Price + t.ServiceTaskParts.Sum(p => p.Quantity * p.UnitPrice)) ?? x.TotalAmount,
            Note = x.Note,
            ServiceOrderId = x.ServiceOrderId,
            CustomerName = x.ServiceOrder?.Car?.Customer != null
                ? $"{x.ServiceOrder.Car.Customer.FirstName} {x.ServiceOrder.Car.Customer.LastName}"
                : null,
            CarDisplay = x.ServiceOrder?.Car != null
                ? $"{x.ServiceOrder.Car.Brand} {x.ServiceOrder.Car.Model} ({x.ServiceOrder.Car.LicensePlate})"
                : null,
            Tasks = x.ServiceOrder?.Tasks.Select(t => new ServiceTaskDto
            {
                Id = t.Id,
                Name = t.Name,
                Price = t.Price,
                ServiceOrderId = t.ServiceOrderId,
                Parts = t.ServiceTaskParts.Select(p => new ServiceTaskPartDto
                {
                    Id = p.Id,
                    ServiceTaskId = p.ServiceTaskId,
                    SparePartId = p.SparePartId,
                    SparePartName = p.SparePart?.Name,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice
                }).ToList()
            }).ToList() ?? new List<ServiceTaskDto>()
        };
    }

    public async Task<Invoice?> GetByIdForDeleteAsync(Guid id) =>
        await _db.Invoices
            .Include(x => x.ServiceOrder)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<bool?> MarkAsPaidAsync(Guid id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice == null) return null;

        invoice.IsPaid = true;
        invoice.PaidAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool?> DeleteAsync(Guid id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice == null) return null;

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();
        return true;
    }
}
