using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.ViewModels;

public class ServiceOrderFormViewModel
{
    public ServiceOrder ServiceOrder { get; set; } = new();
    public IEnumerable<SelectListItem> Cars { get; set; } = Enumerable.Empty<SelectListItem>();
}