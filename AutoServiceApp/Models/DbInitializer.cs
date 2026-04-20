using AutoServiceApp.Models;

namespace AutoServiceApp.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Customers.Any())
        {
            return;
        }

        var customer1 = new Customer
        {
            FirstName = "Tomáš",
            LastName = "Novák",
            Email = "tomas.novak@email.cz",
            Phone = "+420777111222"
        };

        var customer2 = new Customer
        {
            FirstName = "Jan",
            LastName = "Svoboda",
            Email = "jan.svoboda@email.cz",
            Phone = "+420777333444"
        };

        var mechanic1 = new Mechanic
        {
            FirstName = "Petr",
            LastName = "Malý",
            Specialization = "Brzdy a podvozek"
        };

        var mechanic2 = new Mechanic
        {
            FirstName = "Karel",
            LastName = "Dvořák",
            Specialization = "Diagnostika"
        };

        var car1 = new Car
        {
            Brand = "Škoda",
            Model = "Octavia",
            Year = 2018,
            LicensePlate = "1HK1234",
            Customer = customer1
        };

        var car2 = new Car
        {
            Brand = "Volkswagen",
            Model = "Golf",
            Year = 2016,
            LicensePlate = "2HK5678",
            Customer = customer2
        };

        var order1 = new ServiceOrder
        {
            Car = car1,
            Description = "Auto táhne doprava a pískají brzdy.",
            Status = ServiceOrderStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Diagnostika", Price = 800 },
                new() { Name = "Výměna předních destiček", Price = 3200 }
            }
        };
        
        var order2 = new ServiceOrder
        {
            Car = car2,
            Description = "Pravidelný servis a výměna oleje.",
            Status = ServiceOrderStatus.New,
            CreatedAt = DateTime.UtcNow,
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Výměna oleje", Price = 1800 },
                new() { Name = "Výměna filtrů", Price = 1200 }
            }
        };

        db.Customers.AddRange(customer1, customer2);
        db.Mechanics.AddRange(mechanic1, mechanic2);
        db.Cars.AddRange(car1, car2);
        db.ServiceOrders.AddRange(order1, order2);
        db.SaveChanges();

        db.ServiceOrderMechanics.AddRange(
            new ServiceOrderMechanic { ServiceOrderId = order1.Id, MechanicId = mechanic1.Id },
            new ServiceOrderMechanic { ServiceOrderId = order1.Id, MechanicId = mechanic2.Id },
            new ServiceOrderMechanic { ServiceOrderId = order2.Id, MechanicId = mechanic2.Id }
        );

        db.SaveChanges();
    }
}