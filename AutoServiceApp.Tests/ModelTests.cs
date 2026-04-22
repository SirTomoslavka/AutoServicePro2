using AutoServiceApp.Models;

namespace AutoServiceApp.Tests;

public class ModelTests
{
    [Fact]
    public void Customer_FullName_CombinesFirstAndLast()
    {
        var customer = new Customer { FirstName = "Jan", LastName = "Novák" };
        Assert.Equal("Jan Novák", customer.FullName);
    }

    [Fact]
    public void Mechanic_FullName_CombinesFirstAndLast()
    {
        var mechanic = new Mechanic { FirstName = "Petr", LastName = "Malý" };
        Assert.Equal("Petr Malý", mechanic.FullName);
    }

    [Fact]
    public void Car_DisplayName_ShowsBrandModelPlate()
    {
        var car = new Car { Brand = "Škoda", Model = "Octavia", LicensePlate = "1AB1234" };
        Assert.Equal("Škoda Octavia (1AB1234)", car.DisplayName);
    }

    [Fact]
    public void ServiceOrder_TotalPrice_SumsTaskPrices()
    {
        var order = new ServiceOrder
        {
            Description = "Test",
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Task1", Price = 500 },
                new() { Name = "Task2", Price = 1500 }
            }
        };

        Assert.Equal(2000m, order.TotalPrice);
    }

    [Fact]
    public void ServiceOrder_TotalPrice_IncludesPartsCost()
    {
        var order = new ServiceOrder
        {
            Description = "Test",
            Tasks = new List<ServiceTask>
            {
                new()
                {
                    Name = "Výměna oleje",
                    Price = 500,
                    ServiceTaskParts = new List<ServiceTaskPart>
                    {
                        new() { Quantity = 5, UnitPrice = 200 }   // 1 000 Kč v dílech
                    }
                }
            }
        };

        Assert.Equal(1500m, order.TotalPrice); // 500 práce + 1 000 díly
    }

    [Fact]
    public void ServiceOrder_TotalPrice_MultipleTasksWithParts()
    {
        var order = new ServiceOrder
        {
            Description = "Test",
            Tasks = new List<ServiceTask>
            {
                new()
                {
                    Name = "Task1", Price = 300,
                    ServiceTaskParts = new List<ServiceTaskPart>
                    {
                        new() { Quantity = 2, UnitPrice = 100 }   // 200
                    }
                },
                new()
                {
                    Name = "Task2", Price = 700,
                    ServiceTaskParts = new List<ServiceTaskPart>
                    {
                        new() { Quantity = 3, UnitPrice = 50 },   // 150
                        new() { Quantity = 1, UnitPrice = 800 }   // 800
                    }
                }
            }
        };

        // 300+200 + 700+150+800 = 2 150
        Assert.Equal(2150m, order.TotalPrice);
    }

    [Fact]
    public void ServiceOrder_TotalPrice_NoTasks_ReturnsZero()
    {
        var order = new ServiceOrder { Description = "Test" };
        Assert.Equal(0m, order.TotalPrice);
    }

    [Fact]
    public void ServiceTaskPart_TotalPrice_CalculatesCorrectly()
    {
        var stp = new ServiceTaskPart { Quantity = 3, UnitPrice = 150 };
        Assert.Equal(450m, stp.TotalPrice);
    }

    [Fact]
    public void ServiceOrderStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)ServiceOrderStatus.New);
        Assert.Equal(1, (int)ServiceOrderStatus.InProgress);
        Assert.Equal(2, (int)ServiceOrderStatus.Done);
        Assert.Equal(3, (int)ServiceOrderStatus.Cancelled);
    }

    [Fact]
    public void Invoice_DefaultValues_AreCorrect()
    {
        var invoice = new Invoice();
        Assert.False(invoice.IsPaid);
        Assert.Null(invoice.PaidAt);
        Assert.Equal(string.Empty, invoice.InvoiceNumber);
    }

    [Fact]
    public void ApplicationUser_FullName_CombinesFirstAndLast()
    {
        var user = new ApplicationUser { FirstName = "Admin", LastName = "User" };
        Assert.Equal("Admin User", user.FullName);
    }
}
