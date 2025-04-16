using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Motorcycle.Domain.Entities;

namespace Motorcycle.Infrastructure.Data.Configurations;

public class DeliveryPersonConfiguration : IEntityTypeConfiguration<DeliveryPersonEntity>
{
    public void Configure(EntityTypeBuilder<DeliveryPersonEntity> builder)
    {
        builder.ToTable("DeliveryPersons");

        // Configuração da chave primária
        builder.HasKey(d => d.Id);

        // Configurações básicas
        builder.Property(d => d.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.BirthDate)
            .IsRequired();

        // Valor de objeto Cnpj
        builder.OwnsOne(d => d.Cnpj, cnpj =>
        {
            cnpj.Property(c => c.Value)
                .HasColumnName("Cnpj")
                .HasMaxLength(18)
                .IsRequired();
            
            // Índice único para garantir que não existam CNPJs duplicados
            cnpj.HasIndex(c => c.Value).IsUnique();
        });

        // Valor de objeto DriverLicense
        builder.OwnsOne(d => d.DriverLicense, dl =>
        {
            dl.Property(l => l.Number)
                .HasColumnName("LicenseNumber")
                .HasMaxLength(11)
                .IsRequired();
            
            dl.Property(l => l.Type)
                .HasColumnName("LicenseType")
                .IsRequired();
            
            dl.Property(l => l.ImagePath)
                .HasColumnName("LicenseImagePath")
                .HasMaxLength(255);
            
            // Índice único para garantir que não existam números de CNH duplicados
            dl.HasIndex(l => l.Number).IsUnique();
        });

        // Relacionamento com RentalEntity
        builder.HasMany(d => d.Rentals)
            .WithOne(r => r.DeliveryPerson)
            .HasForeignKey(r => r.DeliveryPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}