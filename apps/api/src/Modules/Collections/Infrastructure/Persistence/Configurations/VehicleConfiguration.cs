using Api.Modules.Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Modules.Collections.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> entity)
    {
        entity.ToTable("Vehicles");
        entity.HasKey(vehicle => vehicle.Id);
        entity.Property(vehicle => vehicle.Id).ValueGeneratedNever();
        entity.Property(vehicle => vehicle.Plate).HasMaxLength(16).IsRequired();
        entity.HasIndex(vehicle => vehicle.Plate).IsUnique();
        entity.Property(vehicle => vehicle.Description).HasMaxLength(120).IsRequired();
    }
}
