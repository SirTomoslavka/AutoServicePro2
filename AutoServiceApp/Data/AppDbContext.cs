using AutoServiceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoServiceApp.Data;

public class AppDbContext : DbContext
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
    }
}