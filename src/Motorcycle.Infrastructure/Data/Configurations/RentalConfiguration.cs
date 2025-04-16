using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorcycle.Domain.Entities;

namespace Motorcycle.Infrastructure.Data.Configurations;

public class RentalConfiguration : IEntityTypeConfiguration<RentalEntity>
{
    public void Configure(EntityTypeBuilder<RentalEntity> builder)
    {
        builder.ToTable("Rentals");

        // Configuração da chave primária
        builder.HasKey(r => r.Id);

        // Configurações básicas
        builder.Property(r => r.MotorcycleId)
            .IsRequired();

        builder.Property(r => r.DeliveryPersonId)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Valor de objeto RentalPeriod
        builder.OwnsOne(r => r.Period, p =>
        {
            p.Property(x => x.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();
            
            p.Property(x => x.ExpectedEndDate)
                .HasColumnName("ExpectedEndDate")
                .IsRequired();
            
            p.Property(x => x.ActualEndDate)
                .HasColumnName("ActualEndDate");
            
            p.Property(x => x.PlanType)
                .HasColumnName("PlanType")
                .IsRequired();
        });

        // Valor de objeto Money para DailyRate
        builder.OwnsOne(r => r.DailyRate, m =>
        {
            m.Property(x => x.Amount)
                .HasColumnName("DailyRate")
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            
            m.Property(x => x.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Valor de objeto Money para TotalAmount
        builder.OwnsOne(r => r.TotalAmount, m =>
        {
            m.Property(x => x.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)");
            
            m.Property(x => x.Currency)
                .HasColumnName("TotalAmountCurrency")
                .HasMaxLength(3);
        });

        // Relacionamentos
        builder.HasOne(r => r.Motorcycle)
            .WithMany(m => m.Rentals)
            .HasForeignKey(r => r.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.DeliveryPerson)
            .WithMany(d => d.Rentals)
            .HasForeignKey(r => r.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}