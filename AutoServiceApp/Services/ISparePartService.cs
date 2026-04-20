using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface ISparePartService
{
    Task<IList<SparePart>> GetAllAsync();
    Task<SparePart?> GetByIdAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(SparePart sparePart);
    Task UpdateAsync(SparePart sparePart);
    /// <returns>null = not found, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
