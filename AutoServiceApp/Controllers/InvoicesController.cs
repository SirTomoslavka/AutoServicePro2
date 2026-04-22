using AutoServiceApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoServiceApp.Controllers;

[Authorize]
public class InvoicesController : Controller
{
    private readonly IInvoiceService _invoices;
    private readonly IInvoicePdfService _pdf;

    public InvoicesController(IInvoiceService invoices, IInvoicePdfService pdf)
    {
        _invoices = invoices;
        _pdf = pdf;
    }

    public async Task<IActionResult> Index() => View(await _invoices.GetAllAsync());

    public async Task<IActionResult> Detail(Guid id)
    {
        var invoice = await _invoices.GetByIdAsync(id);
        return invoice == null ? NotFound() : View(invoice);
    }

    public async Task<IActionResult> Pdf(Guid id)
    {
        var invoice = await _invoices.GetByIdAsync(id);
        if (invoice == null) return NotFound();

        var pdfBytes = await _pdf.GeneratePdfAsync(invoice);
        return File(pdfBytes, "application/pdf");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(Guid id)
    {
        var result = await _invoices.MarkAsPaidAsync(id);
        return result == null ? NotFound() : RedirectToAction(nameof(Detail), new { id });
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var invoice = await _invoices.GetByIdForDeleteAsync(id);
        return invoice == null ? NotFound() : View(invoice);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var result = await _invoices.DeleteAsync(id);
        return result == null ? NotFound() : RedirectToAction(nameof(Index));
    }
}
