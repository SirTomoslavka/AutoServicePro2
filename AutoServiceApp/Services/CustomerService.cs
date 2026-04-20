using AutoServiceApp.Data;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db) => _db = db;

    public async Task<IList<Customer>> GetAllAsync() =>
        await _db.Customers
            .Include(x => x.Cars)
            .OrderBy(x => x.LastName)
            .ToListAsync();

    public async Task<Customer?> GetByIdAsync(Guid id) =>
        await _db.Customers
            .Include(x => x.Cars)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IList<SelectListItem>> GetSelectListAsync() =>
        await _db.Customers
            .OrderBy(x => x.LastName)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName
            })
            .ToListAsync();

    public async Task CreateAsync(Customer customer)
    {
        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(Guid id)
    {
        var customer = await _db.Customers
            .Include(x => x.Cars)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (customer == null) return null;
        if (customer.Cars.Any()) return false;

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        return true;
    }
}
