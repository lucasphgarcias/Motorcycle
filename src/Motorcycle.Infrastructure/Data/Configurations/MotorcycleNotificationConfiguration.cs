using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorcycle.Infrastructure.Data.Entities;

namespace Motorcycle.Infrastructure.Data.Configurations;

public class MotorcycleNotificationConfiguration : IEntityTypeConfiguration<MotorcycleNotification>
{
    public void Configure(EntityTypeBuilder<MotorcycleNotification> builder)
    {
        builder.ToTable("MotorcycleNotifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.MotorcycleId)
            .IsRequired();

        builder.Property(n => n.LicensePlate)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(n => n.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.Year)
            .IsRequired();

        builder.Property(n => n.NotificationTimestamp)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();
    }
} 