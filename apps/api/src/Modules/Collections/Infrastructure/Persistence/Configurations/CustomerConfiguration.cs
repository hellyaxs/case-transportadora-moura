using Api.Modules.Collections.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Modules.Collections.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.ToTable("Customers");
        entity.HasKey(customer => customer.Id);
        entity.Property(customer => customer.Id).ValueGeneratedNever();
        entity.Property(customer => customer.Name).HasMaxLength(160).IsRequired();
    }
}
