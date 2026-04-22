using AutoServiceApp.Dtos;

namespace AutoServiceApp.Services;

public interface IInvoicePdfService
{
    Task<byte[]> GeneratePdfAsync(InvoiceDto invoice);
}
