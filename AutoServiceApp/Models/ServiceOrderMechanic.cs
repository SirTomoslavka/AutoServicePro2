namespace AutoServiceApp.Models;

public class ServiceOrderMechanic
{
    public Guid ServiceOrderId { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }

    public Guid MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }
}