using Api.Modules.Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Modules.Collections.Infrastructure.Persistence.Configurations;

public sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> entity)
    {
        entity.ToTable("Drivers");
        entity.HasKey(driver => driver.Id);
        entity.Property(driver => driver.Id).ValueGeneratedNever();
        entity.Property(driver => driver.Name).HasMaxLength(160).IsRequired();
    }
}
