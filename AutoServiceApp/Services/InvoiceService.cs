using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;

    public InvoiceService(AppDbContext db) => _db = db;

    public async Task<IList<Invoice>> GetAllAsync() =>
        await _db.Invoices
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x!.Car)
                    .ThenInclude(x => x!.Customer)
            .OrderByDescending(x => x.IssuedAt)
            .ToListAsync();

    public async Task<Invoice?> GetByIdAsync(Guid id) =>
        await _db.Invoices
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x!.Car)
                    .ThenInclude(x => x!.Customer)
            .Include(x => x.ServiceOrder)
                .ThenInclude(x => x!.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id);

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
