using AutoServiceApp.Dtos;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace AutoServiceApp.Services;

public class InvoicePdfService : IInvoicePdfService
{
    public async Task<byte[]> GeneratePdfAsync(InvoiceDto invoice)
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
        });

        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(BuildHtml(invoice));

        return await page.PdfDataAsync(new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            MarginOptions = new MarginOptions
            {
                Top = "15mm",
                Bottom = "15mm",
                Left = "15mm",
                Right = "15mm"
            }
        });
    }

    private static string BuildHtml(InvoiceDto inv)
    {
        var paidBadge = inv.IsPaid
            ? "<span style='color:#198754;font-weight:bold;'>✔ Zaplaceno</span>"
            : "<span style='color:#dc3545;font-weight:bold;'>✗ Nezaplaceno</span>";

        var paidDate = inv.PaidAt.HasValue
            ? $"<p><strong>Datum úhrady:</strong> {inv.PaidAt.Value.ToLocalTime():dd.MM.yyyy}</p>"
            : string.Empty;

        var rows = string.Join("\n", inv.Tasks.SelectMany(t =>
        {
            var taskRow = $"<tr><td>{System.Net.WebUtility.HtmlEncode(t.Name)}</td><td style='text-align:right;'>{t.Price:N2} Kč</td></tr>";
            var partRows = t.Parts.Select(p =>
                $"<tr style='color:#555;font-size:12px;'><td style='padding-left:24px;'>↳ {System.Net.WebUtility.HtmlEncode(p.SparePartName ?? "Díl")} × {p.Quantity}</td><td style='text-align:right;'>{p.TotalPrice:N2} Kč</td></tr>");
            return new[] { taskRow }.Concat(partRows);
        }));

        var grandTotal = inv.TotalAmount;

        var note = !string.IsNullOrWhiteSpace(inv.Note)
            ? $"<p><strong>Poznámka:</strong> {System.Net.WebUtility.HtmlEncode(inv.Note)}</p>"
            : string.Empty;

        return $$"""
<!DOCTYPE html>
<html lang="cs">
<head>
<meta charset="utf-8" />
<title>Faktura {{System.Net.WebUtility.HtmlEncode(inv.InvoiceNumber)}}</title>
<style>
  body { font-family: Arial, sans-serif; font-size: 13px; color: #222; }
  h1 { font-size: 22px; margin-bottom: 4px; }
  .header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 24px; }
  .company { font-size: 18px; font-weight: bold; color: #0d6efd; }
  table { width: 100%; border-collapse: collapse; margin-top: 16px; }
  th { background: #0d6efd; color: #fff; padding: 8px; text-align: left; }
  td { padding: 7px 8px; border-bottom: 1px solid #dee2e6; }
  tfoot td { font-weight: bold; font-size: 15px; background: #f8f9fa; }
  .meta { display: flex; gap: 40px; margin-bottom: 20px; }
  .meta div { flex: 1; }
  hr { border: none; border-top: 1px solid #dee2e6; margin: 20px 0; }
</style>
</head>
<body>
<div class="header">
  <div>
    <div class="company">AutoService</div>
    <div>autoservis@test.cz</div>
  </div>
  <div style="text-align:right;">
    <h1>Faktura</h1>
    <div style="font-size:15px;font-weight:bold;">{{System.Net.WebUtility.HtmlEncode(inv.InvoiceNumber)}}</div>
    <div>{{paidBadge}}</div>
  </div>
</div>
<hr/>
<div class="meta">
  <div>
    <p><strong>Zákazník:</strong> {{System.Net.WebUtility.HtmlEncode(inv.CustomerName ?? "—")}}</p>
    <p><strong>Auto:</strong> {{System.Net.WebUtility.HtmlEncode(inv.CarDisplay ?? "—")}}</p>
    {{note}}
  </div>
  <div style="text-align:right;">
    <p><strong>Datum vystavení:</strong> {{inv.IssuedAt.ToLocalTime():dd.MM.yyyy}}</p>
    {{paidDate}}
  </div>
</div>
<table>
  <thead>
    <tr><th>Úkon</th><th style="text-align:right;">Cena</th></tr>
  </thead>
  <tbody>
    {{rows}}
  </tbody>
  <tfoot>
    <tr>
      <td>Celkem</td>
      <td style="text-align:right;">{{grandTotal:N2}} Kč</td>
    </tr>
  </tfoot>
</table>
</body>
</html>
""";
    }
}
