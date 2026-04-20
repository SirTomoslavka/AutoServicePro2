using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface ICustomerService
{
    Task<IList<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    /// <returns>null = not found, false = has associated cars, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
