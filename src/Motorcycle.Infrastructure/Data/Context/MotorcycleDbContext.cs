using Microsoft.EntityFrameworkCore;
using Motorcycle.Domain.Entities;
using Motorcycle.Infrastructure.Data.Configurations;
using Motorcycle.Infrastructure.Data.Entities; // Adicione esta linha

namespace Motorcycle.Infrastructure.Data.Context;

public class MotorcycleDbContext : DbContext
{
    public DbSet<MotorcycleEntity> Motorcycles { get; set; } = null!;
    public DbSet<DeliveryPersonEntity> DeliveryPersons { get; set; } = null!;
    public DbSet<RentalEntity> Rentals { get; set; } = null!;
    public DbSet<Motorcycle2024Notification> Motorcycle2024Notifications { get; set; } = null!;
    public DbSet<MotorcycleNotification> MotorcycleNotifications { get; set; } = null!;

    public MotorcycleDbContext(DbContextOptions<MotorcycleDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configurações
        modelBuilder.ApplyConfiguration(new MotorcycleConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryPersonConfiguration());
        modelBuilder.ApplyConfiguration(new RentalConfiguration());
        modelBuilder.ApplyConfiguration(new Motorcycle2024NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new MotorcycleNotificationConfiguration());
    }
}