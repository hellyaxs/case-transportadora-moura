using Api.Modules.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Modules.Auth.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users");
        entity.HasKey(user => user.Id);
        entity.Property(user => user.Email).HasMaxLength(160).IsRequired();
        entity.Property(user => user.Name).HasMaxLength(120).IsRequired();
        entity.Property(user => user.PasswordHash).HasMaxLength(200).IsRequired();
        entity.HasIndex(user => user.Email).IsUnique();
    }
}
