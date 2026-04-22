using AutoServiceApp.Dtos;
using AutoServiceApp.Models;

namespace AutoServiceApp.Services;

public interface IServiceOrderService
{
    Task<IList<ServiceOrderDto>> GetAllAsync(ServiceOrderStatus? status = null);
    Task<ServiceOrderDto?> GetByIdAsync(Guid id);
    Task<ServiceOrder?> GetByIdForEditAsync(Guid id);
    Task<ServiceOrder?> GetByIdForDeleteAsync(Guid id);
    Task<ServiceOrder?> GetByIdWithMechanicsAsync(Guid id);
    Task CreateAsync(ServiceOrder serviceOrder);
    Task UpdateAsync(ServiceOrder serviceOrder);
    /// <returns>null = not found, true = deleted</returns>
    Task<bool?> DeleteAsync(Guid id);
    Task<bool> AddTaskAsync(Guid serviceOrderId, string name, decimal price);
    /// <returns>null = not found, true = updated</returns>
    Task<bool?> AssignMechanicsAsync(Guid serviceOrderId, IList<Guid> mechanicIds);
    /// <returns>null = not found, true = updated</returns>
    Task<bool?> ChangeStatusAsync(Guid id, ServiceOrderStatus newStatus);
    /// <returns>null = order not found, otherwise the invoice (existing or newly created)</returns>
    Task<Invoice?> GenerateInvoiceAsync(Guid serviceOrderId);
    /// <returns>null = spare part not found, true = added/updated</returns>
    Task<bool?> AddPartToTaskAsync(Guid serviceTaskId, Guid sparePartId, int quantity);
    Task RemovePartFromTaskAsync(Guid serviceTaskPartId);
}
