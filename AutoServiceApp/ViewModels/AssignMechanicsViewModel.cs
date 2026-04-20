using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoServiceApp.ViewModels;

public class AssignMechanicsViewModel
{
    public Guid ServiceOrderId { get; set; }

    [Display(Name = "Mechanici")]
    public List<Guid> SelectedMechanicIds { get; set; } = new();

    public IEnumerable<SelectListItem> Mechanics { get; set; } = Enumerable.Empty<SelectListItem>();
}