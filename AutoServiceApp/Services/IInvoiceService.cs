using AutoServiceApp.Models;

namespace AutoServiceApp.Services;

public interface IInvoiceService
{
    Task<IList<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<Invoice?> GetByIdForDeleteAsync(Guid id);
    /// <returns>null = not found, true = marked paid</returns>
    Task<bool?> MarkAsPaidAsync(Guid id);
    /// <returns>null = not found, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
}
