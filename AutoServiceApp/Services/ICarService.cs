using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface ICarService
{
    Task<IList<Car>> GetAllAsync();
    Task<Car?> GetByIdAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(Car car);
    Task UpdateAsync(Car car);
    /// <returns>null = not found, false = has service orders, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
