using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface IMechanicService
{
    Task<IList<MechanicDto>> GetAllAsync();
    Task<MechanicDto?> GetByIdAsync(Guid id);
    Task<Mechanic?> GetByIdForEditAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(Mechanic mechanic);
    Task UpdateAsync(Mechanic mechanic);
    /// <returns>null = not found, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
