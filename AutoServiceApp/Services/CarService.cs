using AutoServiceApp.Data;
using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Services;

public class CarService : ICarService
{
    private readonly AppDbContext _db;

    public CarService(AppDbContext db) => _db = db;

    public async Task<IList<CarDto>> GetAllAsync() =>
        await _db.Cars
            .Include(x => x.Customer)
            .OrderBy(x => x.Brand)
            .ThenBy(x => x.Model)
            .Select(x => new CarDto
            {
                Id = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Year = x.Year,
                LicensePlate = x.LicensePlate,
                CustomerId = x.CustomerId,
                CustomerName = x.Customer != null ? x.Customer.FirstName + " " + x.Customer.LastName : null
            })
            .ToListAsync();

    public async Task<CarDto?> GetByIdAsync(Guid id)
    {
        var x = await _db.Cars
            .Include(x => x.Customer)
            .Include(x => x.ServiceOrders)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (x == null) return null;

        return new CarDto
        {
            Id = x.Id,
            Brand = x.Brand,
            Model = x.Model,
            Year = x.Year,
            LicensePlate = x.LicensePlate,
            CustomerId = x.CustomerId,
            CustomerName = x.Customer?.FirstName + " " + x.Customer?.LastName,
            ServiceOrders = x.ServiceOrders.Select(o => new ServiceOrderSummaryDto
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                Description = o.Description
            }).ToList()
        };
    }

    public async Task<Car?> GetByIdForEditAsync(Guid id) =>
        await _db.Cars
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<IList<SelectListItem>> GetSelectListAsync() =>
        await _db.Cars
            .Include(x => x.Customer)
            .OrderBy(x => x.Brand)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Brand + " " + x.Model + " (" + x.LicensePlate + ") - " + x.Customer!.FirstName + " " + x.Customer.LastName
            })
            .ToListAsync();

    public async Task CreateAsync(Car car)
    {
        _db.Cars.Add(car);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Car car)
    {
        _db.Cars.Update(car);
        await _db.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(Guid id)
    {
        var car = await _db.Cars
            .Include(x => x.ServiceOrders)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (car == null) return null;
        if (car.ServiceOrders.Any()) return false;

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync();
        return true;
    }
}
