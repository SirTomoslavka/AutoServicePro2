using AutoServiceApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.ViewModels;

public class CarFormViewModel
{
    public Car Car { get; set; } = new();
    public IEnumerable<SelectListItem> Customers { get; set; } = Enumerable.Empty<SelectListItem>();
}