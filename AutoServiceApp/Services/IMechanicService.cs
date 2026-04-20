using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface IMechanicService
{
    Task<IList<Mechanic>> GetAllAsync();
    Task<Mechanic?> GetByIdAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(Mechanic mechanic);
    Task UpdateAsync(Mechanic mechanic);
    /// <returns>null = not found, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
