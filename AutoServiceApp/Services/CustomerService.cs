using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db) => _db = db;

    public async Task<IList<CustomerDto>> GetAllAsync() =>
        await _db.Customers
            .Include(x => x.Cars)
            .OrderBy(x => x.LastName)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Cars = x.Cars.Select(c => new CarSummaryDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    LicensePlate = c.LicensePlate,
                    Year = c.Year
                }).ToList()
            })
            .ToListAsync();

    public async Task<CustomerDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.Customers
            .Include(x => x.Cars)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (x == null) return null;

        return new CustomerDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Phone = x.Phone,
            Cars = x.Cars.Select(c => new CarSummaryDto
            {
                Id = c.Id,
                Brand = c.Brand,
                Model = c.Model,
                LicensePlate = c.LicensePlate,
                Year = c.Year
            }).ToList()
        };
    }

    public async Task<Customer?> GetByIdForEditAsync(Guid id) =>
        await _db.Customers.FindAsync(id);

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
