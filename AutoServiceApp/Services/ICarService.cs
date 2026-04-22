using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface ICarService
{
    Task<IList<CarDto>> GetAllAsync();
    Task<CarDto?> GetByIdAsync(Guid id);
    Task<Car?> GetByIdForEditAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(Car car);
    Task UpdateAsync(Car car);
    /// <returns>null = not found, false = has service orders, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
