using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorcycle.Domain.Entities;
using Motorcycle.Domain.ValueObjects;

namespace Motorcycle.Infrastructure.Data.Configurations;

public class MotorcycleConfiguration : IEntityTypeConfiguration<MotorcycleEntity>
{
    public void Configure(EntityTypeBuilder<MotorcycleEntity> builder)
    {
        builder.ToTable("Motorcycles");

        // Configuração da chave primária
        builder.HasKey(m => m.Id);

        // Configurações básicas
        builder.Property(m => m.Model)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Year)
            .IsRequired();

        // Valor de objeto LicensePlate
        builder.OwnsOne(m => m.LicensePlate, lp =>
        {
            lp.Property(p => p.Value)
                .HasColumnName("LicensePlate")
                .HasMaxLength(10)
                .IsRequired();
            
            // Índice único para garantir que não existam placas duplicadas
            lp.HasIndex(p => p.Value).IsUnique();
        });

        // Relacionamento com RentalEntity
        builder.HasMany(m => m.Rentals)
            .WithOne(r => r.Motorcycle)
            .HasForeignKey(r => r.MotorcycleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignora as propriedades não mapeadas para o banco de dados
        builder.Ignore(m => m.DomainEvents);
    }
}