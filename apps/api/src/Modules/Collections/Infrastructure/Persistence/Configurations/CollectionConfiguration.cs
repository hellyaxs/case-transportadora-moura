using Api.Modules.Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Modules.Collections.Infrastructure.Persistence.Configurations;

public sealed class CollectionConfiguration : IEntityTypeConfiguration<Collection>
{
    public void Configure(EntityTypeBuilder<Collection> entity)
    {
        entity.ToTable("Collections");
        entity.HasKey(collection => collection.Id);
        entity.Property(collection => collection.Id).ValueGeneratedNever();
        entity.Property(collection => collection.Number).HasMaxLength(32).IsRequired();
        entity.HasIndex(collection => collection.Number).IsUnique();
        entity.HasIndex(collection => collection.Status);
        entity.HasIndex(collection => collection.CustomerId);
        entity.HasIndex(collection => collection.ExpectedPickupDate);
        entity.Property(collection => collection.CustomerName).HasMaxLength(160).IsRequired();
        entity.Property(collection => collection.SenderName).HasMaxLength(160).IsRequired();
        entity.Property(collection => collection.SenderAddress).HasMaxLength(240).IsRequired();
        entity.Property(collection => collection.RecipientName).HasMaxLength(160).IsRequired();
        entity.Property(collection => collection.RecipientAddress).HasMaxLength(240).IsRequired();
        entity.Property(collection => collection.Priority).HasConversion<string>().HasMaxLength(24).IsRequired();
        entity.Property(collection => collection.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
        entity.Property(collection => collection.Notes).HasMaxLength(500);
        entity.Property(collection => collection.DriverName).HasMaxLength(160);
        entity.Property(collection => collection.VehiclePlate).HasMaxLength(16);
        entity.Property(collection => collection.CancellationReason).HasMaxLength(300);
        entity.HasMany(collection => collection.Incidents)
            .WithOne()
            .HasForeignKey(incident => incident.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);
        entity.Navigation(collection => collection.Incidents).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
