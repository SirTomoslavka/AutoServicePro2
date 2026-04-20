using AutoServiceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<ServiceTask> ServiceTasks => Set<ServiceTask>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<ServiceOrderMechanic> ServiceOrderMechanics => Set<ServiceOrderMechanic>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<SparePart> SpareParts => Set<SparePart>();
    public DbSet<ServiceTaskPart> ServiceTaskParts => Set<ServiceTaskPart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ServiceOrderMechanic>()
            .HasKey(x => new { x.ServiceOrderId, x.MechanicId });

        modelBuilder.Entity<ServiceOrderMechanic>()
            .HasOne(x => x.ServiceOrder)
            .WithMany(x => x.ServiceOrderMechanics)
            .HasForeignKey(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceOrderMechanic>()
            .HasOne(x => x.Mechanic)
            .WithMany(x => x.ServiceOrderMechanics)
            .HasForeignKey(x => x.MechanicId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Car>()
            .HasIndex(x => x.LicensePlate)
            .IsUnique();

        modelBuilder.Entity<ServiceTask>()
            .Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Invoice>()
            .HasOne(x => x.ServiceOrder)
            .WithOne(x => x.Invoice)
            .HasForeignKey<Invoice>(x => x.ServiceOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .HasIndex(x => x.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<Invoice>()
            .Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<SparePart>()
            .Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ServiceTaskPart>()
            .Property(x => x.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<ServiceTaskPart>()
            .HasOne(x => x.ServiceTask)
            .WithMany(x => x.ServiceTaskParts)
            .HasForeignKey(x => x.ServiceTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ServiceTaskPart>()
            .HasOne(x => x.SparePart)
            .WithMany(x => x.ServiceTaskParts)
            .HasForeignKey(x => x.SparePartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}