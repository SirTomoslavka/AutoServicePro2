using AutoServiceApp.Dtos;
using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.Services;

public interface ICustomerService
{
    Task<IList<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(Guid id);
    Task<Customer?> GetByIdForEditAsync(Guid id);
    Task<IList<SelectListItem>> GetSelectListAsync();
    Task CreateAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    /// <returns>null = not found, false = has associated cars, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
