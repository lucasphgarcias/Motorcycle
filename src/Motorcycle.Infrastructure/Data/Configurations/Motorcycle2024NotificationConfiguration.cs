using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorcycle.Infrastructure.Data.Entities;

namespace Motorcycle.Infrastructure.Data.Configurations;

public class Motorcycle2024NotificationConfiguration : IEntityTypeConfiguration<Motorcycle2024Notification>
{
    public void Configure(EntityTypeBuilder<Motorcycle2024Notification> builder)
    {
        builder.ToTable("Motorcycle2024Notifications");

        // Configuração da chave primária
        builder.HasKey(n => n.Id);

        // Configurações básicas
        builder.Property(n => n.MotorcycleId)
            .IsRequired();

        builder.Property(n => n.LicensePlate)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(n => n.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.NotificationTimestamp)
            .IsRequired();

        builder.Property(n => n.CreatedAt)
            .IsRequired();
    }
}