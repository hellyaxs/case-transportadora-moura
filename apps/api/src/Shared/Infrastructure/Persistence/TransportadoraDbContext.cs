using Api.Modules.Auth.Domain.Entities;
using Api.Modules.Auth.Infrastructure.Persistence.Configurations;
using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Api.Shared.Infrastructure.Persistence;

public sealed class TransportadoraDbContext(DbContextOptions<TransportadoraDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Collection> Collections => Set<Collection>();
    public DbSet<CollectionIncident> CollectionIncidents => Set<CollectionIncident>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new DriverConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new CollectionConfiguration());
        modelBuilder.ApplyConfiguration(new CollectionIncidentConfiguration());
    }
}
