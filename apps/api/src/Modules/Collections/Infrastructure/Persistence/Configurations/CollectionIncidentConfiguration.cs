using Api.Modules.Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Modules.Collections.Infrastructure.Persistence.Configurations;

public sealed class CollectionIncidentConfiguration : IEntityTypeConfiguration<CollectionIncident>
{
    public void Configure(EntityTypeBuilder<CollectionIncident> entity)
    {
        entity.ToTable("CollectionIncidents");
        entity.HasKey(incident => incident.Id);
        entity.Property(incident => incident.Id).ValueGeneratedNever();
        entity.HasIndex(incident => incident.CollectionId);
        entity.Property(incident => incident.Description).HasMaxLength(500).IsRequired();
        entity.Property(incident => incident.ResponsibleUser).HasMaxLength(120).IsRequired();
        entity.Property(incident => incident.RegisteredAt).IsRequired();
    }
}
