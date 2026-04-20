using AutoServiceApp.Models;

namespace AutoServiceApp.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        if (db.Customers.Any()) return;

        // ── Customers ────────────────────────────────────────────────────────────
        var customers = new[]
        {
            new Customer { FirstName = "Tomáš",   LastName = "Novák",     Email = "tomas.novak@email.cz",     Phone = "+420777111222" },
            new Customer { FirstName = "Jan",      LastName = "Svoboda",   Email = "jan.svoboda@email.cz",     Phone = "+420777333444" },
            new Customer { FirstName = "Markéta",  LastName = "Horáková",  Email = "marketa.horakova@gmail.com", Phone = "+420606123456" },
            new Customer { FirstName = "Lukáš",   LastName = "Procházka", Email = "lukas.prochazka@seznam.cz", Phone = "+420725987654" },
            new Customer { FirstName = "Petra",    LastName = "Nováčková", Email = "petra.novackova@email.cz",  Phone = "+420731456789" },
            new Customer { FirstName = "Ondřej",  LastName = "Blažek",    Email = "ondrej.blazek@gmail.com",   Phone = "+420602345678" },
            new Customer { FirstName = "Veronika", LastName = "Marková",   Email = "veronika.markova@email.cz", Phone = "+420773654321" },
            new Customer { FirstName = "Martin",   LastName = "Šimánek",   Email = "martin.simanek@firma.cz",   Phone = "+420776111333" },
            new Customer { FirstName = "Lenka",    LastName = "Čermáková", Email = "lenka.cermakova@email.cz",  Phone = "+420605777888" },
            new Customer { FirstName = "Roman",    LastName = "Krejčí",    Email = "roman.krejci@email.cz",     Phone = "+420721222333" },
        };
        db.Customers.AddRange(customers);

        // ── Mechanics ────────────────────────────────────────────────────────────
        var mech1  = new Mechanic { FirstName = "Petr",    LastName = "Malý",      Specialization = "Brzdy a podvozek" };
        var mech2  = new Mechanic { FirstName = "Karel",   LastName = "Dvořák",    Specialization = "Diagnostika a elektronika" };
        var mech3  = new Mechanic { FirstName = "Jiří",    LastName = "Kratochvíl",Specialization = "Motor a převodovka" };
        var mech4  = new Mechanic { FirstName = "Radek",   LastName = "Pospíšil",  Specialization = "Klimatizace a elektroinstalace" };
        var mech5  = new Mechanic { FirstName = "Zdeněk",  LastName = "Vávra",     Specialization = "Karoserie a lakování" };
        db.Mechanics.AddRange(mech1, mech2, mech3, mech4, mech5);

        // ── Spare parts ──────────────────────────────────────────────────────────
        var pBrakes     = new SparePart { Name = "Brzdové destičky přední", CatalogNumber = "BD-001",  UnitPrice = 850,  StockQuantity = 12 };
        var pOilFilter  = new SparePart { Name = "Olejový filtr",           CatalogNumber = "OF-100",  UnitPrice = 250,  StockQuantity = 25 };
        var pAirFilter  = new SparePart { Name = "Vzduchový filtr",         CatalogNumber = "VF-200",  UnitPrice = 320,  StockQuantity = 18 };
        var pOil5w30    = new SparePart { Name = "Motorový olej 5W-30 (1L)",CatalogNumber = "MO-530",  UnitPrice = 280,  StockQuantity = 40 };
        var pSparkPlug  = new SparePart { Name = "Zapalovací svíčka",       CatalogNumber = "ZS-010",  UnitPrice = 120,  StockQuantity = 50 };
        var pBeltTiming = new SparePart { Name = "Rozvodový řemen",         CatalogNumber = "RR-300",  UnitPrice = 1800, StockQuantity = 8  };
        var pCoolant    = new SparePart { Name = "Chladicí kapalina (1L)",  CatalogNumber = "CK-050",  UnitPrice = 150,  StockQuantity = 30 };
        var pBrakeFluid = new SparePart { Name = "Brzdová kapalina (0,5L)", CatalogNumber = "BK-020",  UnitPrice = 180,  StockQuantity = 20 };
        var pBrakeDrum  = new SparePart { Name = "Brzdový buben zadní",     CatalogNumber = "BB-002",  UnitPrice = 2200, StockQuantity = 5  };
        var pWiperBladeF= new SparePart { Name = "Stěrač přední (1 ks)",    CatalogNumber = "ST-110",  UnitPrice = 390,  StockQuantity = 22 };
        var pBattery    = new SparePart { Name = "Autobaterie 60Ah",        CatalogNumber = "AB-060",  UnitPrice = 2800, StockQuantity = 6  };
        var pCvBoot     = new SparePart { Name = "Manžeta poloosy",         CatalogNumber = "MP-400",  UnitPrice = 650,  StockQuantity = 14 };
        var pShockAbsorb= new SparePart { Name = "Tlumič pérování přední",  CatalogNumber = "TP-500",  UnitPrice = 3500, StockQuantity = 4  };
        var pFuelFilter = new SparePart { Name = "Palivový filtr",          CatalogNumber = "PF-800",  UnitPrice = 420,  StockQuantity = 15 };
        db.SpareParts.AddRange(pBrakes, pOilFilter, pAirFilter, pOil5w30, pSparkPlug, pBeltTiming,
                               pCoolant, pBrakeFluid, pBrakeDrum, pWiperBladeF, pBattery, pCvBoot,
                               pShockAbsorb, pFuelFilter);

        // ── Cars ─────────────────────────────────────────────────────────────────
        var carNovak1 = new Car { Brand = "Škoda",      Model = "Octavia",    Year = 2018, LicensePlate = "1HK1234", Customer = customers[0] };
        var carNovak2 = new Car { Brand = "Škoda",      Model = "Fabia",      Year = 2015, LicensePlate = "1HK5555", Customer = customers[0] };
        var carSvob   = new Car { Brand = "Volkswagen", Model = "Golf",       Year = 2016, LicensePlate = "2HK5678", Customer = customers[1] };
        var carHorak  = new Car { Brand = "Ford",       Model = "Focus",      Year = 2019, LicensePlate = "3AB2345", Customer = customers[2] };
        var carProch  = new Car { Brand = "Toyota",     Model = "Corolla",    Year = 2020, LicensePlate = "4CD3456", Customer = customers[3] };
        var carNovac  = new Car { Brand = "BMW",        Model = "320d",       Year = 2017, LicensePlate = "5EF4567", Customer = customers[4] };
        var carBlaz   = new Car { Brand = "Hyundai",    Model = "i30",        Year = 2021, LicensePlate = "6GH5678", Customer = customers[5] };
        var carMark   = new Car { Brand = "Renault",    Model = "Megane",     Year = 2014, LicensePlate = "7IJ6789", Customer = customers[6] };
        var carSiman  = new Car { Brand = "Audi",       Model = "A4",         Year = 2019, LicensePlate = "8KL7890", Customer = customers[7] };
        var carSiman2 = new Car { Brand = "Mercedes",   Model = "C 200",      Year = 2022, LicensePlate = "8KL7891", Customer = customers[7] };
        var carCerm   = new Car { Brand = "Peugeot",    Model = "308",        Year = 2016, LicensePlate = "9MN8901", Customer = customers[8] };
        var carKrejc  = new Car { Brand = "Kia",        Model = "Ceed",       Year = 2020, LicensePlate = "0OP9012", Customer = customers[9] };
        db.Cars.AddRange(carNovak1, carNovak2, carSvob, carHorak, carProch, carNovac,
                         carBlaz, carMark, carSiman, carSiman2, carCerm, carKrejc);

        // ── Service orders ───────────────────────────────────────────────────────
        // Helper: tasks with optional parts
        var t1a = new ServiceTask { Name = "Diagnostika",                  Price = 800  };
        var t1b = new ServiceTask { Name = "Výměna předních brzdových destiček", Price = 3200 };
        var order1 = new ServiceOrder
        {
            Car = carNovak1, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-90),
            Description = "Auto táhne doprava, pískají brzdy.",
            Tasks = new List<ServiceTask> { t1a, t1b }
        };

        var t2a = new ServiceTask { Name = "Výměna oleje",    Price = 1800 };
        var t2b = new ServiceTask { Name = "Výměna filtrů",   Price = 1200 };
        var order2 = new ServiceOrder
        {
            Car = carSvob, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-75),
            Description = "Pravidelný servis – výměna oleje a filtrů.",
            Tasks = new List<ServiceTask> { t2a, t2b }
        };

        var t3a = new ServiceTask { Name = "Výměna rozvodového řemene", Price = 4500 };
        var t3b = new ServiceTask { Name = "Výměna vodní pumpy",        Price = 2200 };
        var order3 = new ServiceOrder
        {
            Car = carHorak, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-60),
            Description = "Preventivní výměna rozvodového řemene a vodní pumpy.",
            Tasks = new List<ServiceTask> { t3a, t3b }
        };

        var t4a = new ServiceTask { Name = "Výměna zapalovacích svíček", Price = 950  };
        var t4b = new ServiceTask { Name = "Čištění vstřikovačů",        Price = 1600 };
        var order4 = new ServiceOrder
        {
            Car = carProch, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-50),
            Description = "Motor se trhavě startuje, vyšší spotřeba paliva.",
            Tasks = new List<ServiceTask> { t4a, t4b }
        };

        var t5a = new ServiceTask { Name = "Výměna baterie",     Price = 3200 };
        var order5 = new ServiceOrder
        {
            Car = carNovac, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-40),
            Description = "Autobaterie odmítá startovat za mrazu.",
            Tasks = new List<ServiceTask> { t5a }
        };

        var t6a = new ServiceTask { Name = "Diagnostika ABS",            Price = 1100 };
        var t6b = new ServiceTask { Name = "Výměna ABS senzoru",         Price = 2400 };
        var order6 = new ServiceOrder
        {
            Car = carBlaz, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-35),
            Description = "Kontrolka ABS – auto ztrácí funkci ABS při brzdění.",
            Tasks = new List<ServiceTask> { t6a, t6b }
        };

        var t7a = new ServiceTask { Name = "Výměna tlumičů přední nápravy", Price = 7200 };
        var t7b = new ServiceTask { Name = "Geometrie kol",                  Price = 1400 };
        var order7 = new ServiceOrder
        {
            Car = carMark, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-28),
            Description = "Auto poskakuje, špatná přilnavost a hluk z podvozku.",
            Tasks = new List<ServiceTask> { t7a, t7b }
        };

        var t8a = new ServiceTask { Name = "Plnění klimatizace",  Price = 1800 };
        var t8b = new ServiceTask { Name = "Čištění kondenzátoru",Price = 900  };
        var order8 = new ServiceOrder
        {
            Car = carSiman, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            Description = "Klimatizace nechladí – přezimování.",
            Tasks = new List<ServiceTask> { t8a, t8b }
        };

        var t9a = new ServiceTask { Name = "Oprava prasknuté manžety poloosy", Price = 2600 };
        var order9 = new ServiceOrder
        {
            Car = carCerm, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-15),
            Description = "Při zatáčení slyšet cvakání zpod auta.",
            Tasks = new List<ServiceTask> { t9a }
        };

        var t10a = new ServiceTask { Name = "Výměna chladicí kapaliny", Price = 1400 };
        var t10b = new ServiceTask { Name = "Proplach chladicího okruhu",Price = 600  };
        var order10 = new ServiceOrder
        {
            Car = carKrejc, Status = ServiceOrderStatus.Done,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            Description = "Motor přehřívá – nízký stav chladicí kapaliny.",
            Tasks = new List<ServiceTask> { t10a, t10b }
        };

        // Cancelled orders
        var orderC1 = new ServiceOrder
        {
            Car = carNovak2, Status = ServiceOrderStatus.Cancelled,
            CreatedAt = DateTime.UtcNow.AddDays(-80),
            Description = "Zákazník zrušil servis z osobních důvodů.",
            Tasks = new List<ServiceTask> { new() { Name = "Výměna stěračů", Price = 600 } }
        };

        var orderC2 = new ServiceOrder
        {
            Car = carSiman2, Status = ServiceOrderStatus.Cancelled,
            CreatedAt = DateTime.UtcNow.AddDays(-45),
            Description = "Oprava karoserie – zákazník ji nakonec uplatnil přes pojišťovnu.",
            Tasks = new List<ServiceTask> { new() { Name = "Oprava blatníku", Price = 5500 } }
        };

        // In-progress orders
        var t11a = new ServiceTask { Name = "Výměna palivového filtru",   Price = 1100 };
        var t11b = new ServiceTask { Name = "Čištění palivové nádrže",    Price = 1800 };
        var order11 = new ServiceOrder
        {
            Car = carNovak1, Status = ServiceOrderStatus.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            Description = "Problémy se startováním, nestabilní volnoběh.",
            Tasks = new List<ServiceTask> { t11a, t11b }
        };

        var t12a = new ServiceTask { Name = "Diagnostika motoru", Price = 800 };
        var t12b = new ServiceTask { Name = "Výměna vstřikovačů", Price = 5200 };
        var order12 = new ServiceOrder
        {
            Car = carNovac, Status = ServiceOrderStatus.InProgress,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Description = "Kontrolka motoru, vyšší kouřivost výfuku.",
            Tasks = new List<ServiceTask> { t12a, t12b }
        };

        // New orders (just arrived)
        var order13 = new ServiceOrder
        {
            Car = carHorak, Status = ServiceOrderStatus.New,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Description = "Pravidelná roční prohlídka a výměna oleje.",
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Výměna oleje",       Price = 1800 },
                new() { Name = "Kontrola kapalin",    Price = 500  },
                new() { Name = "Výměna vzduchového filtru", Price = 750 }
            }
        };

        var order14 = new ServiceOrder
        {
            Car = carSvob, Status = ServiceOrderStatus.New,
            CreatedAt = DateTime.UtcNow,
            Description = "Prasknuté čelní sklo – žádost o posouzení opravy.",
            Tasks = new List<ServiceTask> { new() { Name = "Výměna čelního skla", Price = 8500 } }
        };

        var order15 = new ServiceOrder
        {
            Car = carKrejc, Status = ServiceOrderStatus.New,
            CreatedAt = DateTime.UtcNow,
            Description = "Skřípání při brzdění – kontrola brzdového systému.",
            Tasks = new List<ServiceTask>
            {
                new() { Name = "Kontrola brzd",                     Price = 600  },
                new() { Name = "Výměna zadních brzdových destiček",  Price = 2800 }
            }
        };

        db.ServiceOrders.AddRange(order1, order2, order3, order4, order5, order6, order7, order8,
                                  order9, order10, orderC1, orderC2, order11, order12, order13, order14, order15);
        db.SaveChanges();

        // ── Mechanic assignments ──────────────────────────────────────────────────
        db.ServiceOrderMechanics.AddRange(
            new ServiceOrderMechanic { ServiceOrderId = order1.Id,   MechanicId = mech1.Id },
            new ServiceOrderMechanic { ServiceOrderId = order1.Id,   MechanicId = mech2.Id },
            new ServiceOrderMechanic { ServiceOrderId = order2.Id,   MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order3.Id,   MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order4.Id,   MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order4.Id,   MechanicId = mech2.Id },
            new ServiceOrderMechanic { ServiceOrderId = order5.Id,   MechanicId = mech4.Id },
            new ServiceOrderMechanic { ServiceOrderId = order6.Id,   MechanicId = mech2.Id },
            new ServiceOrderMechanic { ServiceOrderId = order6.Id,   MechanicId = mech1.Id },
            new ServiceOrderMechanic { ServiceOrderId = order7.Id,   MechanicId = mech1.Id },
            new ServiceOrderMechanic { ServiceOrderId = order7.Id,   MechanicId = mech5.Id },
            new ServiceOrderMechanic { ServiceOrderId = order8.Id,   MechanicId = mech4.Id },
            new ServiceOrderMechanic { ServiceOrderId = order9.Id,   MechanicId = mech1.Id },
            new ServiceOrderMechanic { ServiceOrderId = order10.Id,  MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order11.Id,  MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order11.Id,  MechanicId = mech2.Id },
            new ServiceOrderMechanic { ServiceOrderId = order12.Id,  MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order13.Id,  MechanicId = mech3.Id },
            new ServiceOrderMechanic { ServiceOrderId = order15.Id,  MechanicId = mech1.Id }
        );
        db.SaveChanges();

        // ── Parts on tasks ────────────────────────────────────────────────────────
        db.ServiceTaskParts.AddRange(
            // order1: brake pads + diagnostics
            new ServiceTaskPart { ServiceTaskId = t1b.Id, SparePartId = pBrakes.Id,    Quantity = 1, UnitPrice = pBrakes.UnitPrice    },
            new ServiceTaskPart { ServiceTaskId = t1b.Id, SparePartId = pBrakeFluid.Id, Quantity = 1, UnitPrice = pBrakeFluid.UnitPrice },
            // order2: oil + filters
            new ServiceTaskPart { ServiceTaskId = t2a.Id, SparePartId = pOil5w30.Id,   Quantity = 5, UnitPrice = pOil5w30.UnitPrice   },
            new ServiceTaskPart { ServiceTaskId = t2b.Id, SparePartId = pOilFilter.Id, Quantity = 1, UnitPrice = pOilFilter.UnitPrice  },
            new ServiceTaskPart { ServiceTaskId = t2b.Id, SparePartId = pAirFilter.Id, Quantity = 1, UnitPrice = pAirFilter.UnitPrice  },
            // order3: timing belt
            new ServiceTaskPart { ServiceTaskId = t3a.Id, SparePartId = pBeltTiming.Id, Quantity = 1, UnitPrice = pBeltTiming.UnitPrice },
            // order4: spark plugs
            new ServiceTaskPart { ServiceTaskId = t4a.Id, SparePartId = pSparkPlug.Id,  Quantity = 4, UnitPrice = pSparkPlug.UnitPrice  },
            new ServiceTaskPart { ServiceTaskId = t4b.Id, SparePartId = pFuelFilter.Id, Quantity = 1, UnitPrice = pFuelFilter.UnitPrice  },
            // order5: battery
            new ServiceTaskPart { ServiceTaskId = t5a.Id, SparePartId = pBattery.Id,    Quantity = 1, UnitPrice = pBattery.UnitPrice    },
            // order7: shock absorbers
            new ServiceTaskPart { ServiceTaskId = t7a.Id, SparePartId = pShockAbsorb.Id, Quantity = 2, UnitPrice = pShockAbsorb.UnitPrice },
            // order9: CV boot
            new ServiceTaskPart { ServiceTaskId = t9a.Id, SparePartId = pCvBoot.Id,     Quantity = 1, UnitPrice = pCvBoot.UnitPrice     },
            // order10: coolant
            new ServiceTaskPart { ServiceTaskId = t10a.Id, SparePartId = pCoolant.Id,   Quantity = 3, UnitPrice = pCoolant.UnitPrice    }
        );
        db.SaveChanges();

        // ── Invoices ─────────────────────────────────────────────────────────────
        // Paid invoices for all Done orders
        static string InvNo(int n) => $"FAK-2026-{n:D4}";

        db.Invoices.AddRange(
            new Invoice
            {
                InvoiceNumber = InvNo(1), ServiceOrderId = order1.Id, IssuedAt = order1.CreatedAt.AddDays(3),
                TotalAmount = order1.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order1.CreatedAt.AddDays(5),
                Note = "Uhrazeno převodem"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(2), ServiceOrderId = order2.Id, IssuedAt = order2.CreatedAt.AddDays(1),
                TotalAmount = order2.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order2.CreatedAt.AddDays(2),
                Note = "Uhrazeno v hotovosti"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(3), ServiceOrderId = order3.Id, IssuedAt = order3.CreatedAt.AddDays(2),
                TotalAmount = order3.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order3.CreatedAt.AddDays(7),
                Note = "Uhrazeno kartou"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(4), ServiceOrderId = order4.Id, IssuedAt = order4.CreatedAt.AddDays(1),
                TotalAmount = order4.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order4.CreatedAt.AddDays(3),
                Note = "Uhrazeno převodem"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(5), ServiceOrderId = order5.Id, IssuedAt = order5.CreatedAt.AddDays(1),
                TotalAmount = order5.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order5.CreatedAt.AddDays(4),
                Note = "Uhrazeno v hotovosti"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(6), ServiceOrderId = order6.Id, IssuedAt = order6.CreatedAt.AddDays(2),
                TotalAmount = order6.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order6.CreatedAt.AddDays(6),
                Note = "Uhrazeno kartou"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(7), ServiceOrderId = order7.Id, IssuedAt = order7.CreatedAt.AddDays(1),
                TotalAmount = order7.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order7.CreatedAt.AddDays(2),
                Note = "Uhrazeno převodem"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(8), ServiceOrderId = order8.Id, IssuedAt = order8.CreatedAt.AddDays(1),
                TotalAmount = order8.Tasks.Sum(t => t.Price),
                IsPaid = true, PaidAt = order8.CreatedAt.AddDays(3),
                Note = "Uhrazeno v hotovosti"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(9), ServiceOrderId = order9.Id, IssuedAt = order9.CreatedAt.AddDays(2),
                TotalAmount = order9.Tasks.Sum(t => t.Price),
                IsPaid = false,
                Note = "Čeká na úhradu"
            },
            new Invoice
            {
                InvoiceNumber = InvNo(10), ServiceOrderId = order10.Id, IssuedAt = order10.CreatedAt.AddDays(1),
                TotalAmount = order10.Tasks.Sum(t => t.Price),
                IsPaid = false,
                Note = "Zákazník slíbil úhradu do 14 dnů"
            }
        );
        db.SaveChanges();
    }
}
