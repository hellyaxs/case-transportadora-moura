using Api.Modules.Collections.Domain.Entities;
using Api.Modules.Collections.Domain.Enums;
using Api.Shared.Infrastructure.Persistence;

namespace Api.Modules.Collections.Infrastructure.Persistence;

public static class CollectionSeedData
{
    public static void Seed(TransportadoraDbContext dbContext)
    {
        if (dbContext.Customers.Any())
        {
            return;
        }

        var baseDate = new DateOnly(2026, 5, 28);
        var createdAt = new DateTimeOffset(2026, 5, 28, 9, 0, 0, TimeSpan.Zero);

        var customerMoura = new Customer(Guid.NewGuid(), "Moura Alimentos");
        var customerAcme = new Customer(Guid.NewGuid(), "ACME Distribuição");

        var driverAna = new Driver(Guid.NewGuid(), "Ana Souza");
        var driverBruno = new Driver(Guid.NewGuid(), "Bruno Lima");

        var vehicleVan = new Vehicle(Guid.NewGuid(), "ABC1D23", "Van urbana");
        var vehicleTruck = new Vehicle(Guid.NewGuid(), "XYZ9K88", "Caminhão baú");

        var openHighPriorityCollection = new Collection(
            Guid.NewGuid(),
            "COL-20260528001",
            customerMoura.Id,
            customerMoura.Name,
            "Centro de Distribuição Moura",
            "Rua das Palmeiras, 100",
            "Mercado Central",
            "Av. Brasil, 900",
            baseDate.AddDays(-1),
            CollectionPriority.High,
            "Coleta com janela crítica.",
            createdAt);

        var inProgressCollection = new Collection(
            Guid.NewGuid(),
            "COL-20260528002",
            customerAcme.Id,
            customerAcme.Name,
            "Galpão ACME",
            "Rua Industrial, 45",
            "Loja Norte",
            "Rua Norte, 12",
            baseDate,
            CollectionPriority.Normal,
            "Conferir volumes no embarque.",
            createdAt);
        inProgressCollection.AssignDriverAndVehicle(driverAna.Id, driverAna.Name, vehicleVan.Id, vehicleVan.Plate, createdAt.AddHours(1));
        inProgressCollection.MarkInProgress(createdAt.AddHours(2));
        inProgressCollection.RegisterIncident(
            "Endereço confirmado por telefone com o remetente.",
            "operador.seed",
            createdAt.AddHours(2).AddMinutes(15));

        var collectedCollection = new Collection(
            Guid.NewGuid(),
            "COL-20260527001",
            customerMoura.Id,
            customerMoura.Name,
            "Unidade Sul",
            "Rua Sul, 300",
            "Cliente Final",
            "Av. Atlântica, 88",
            baseDate.AddDays(-1),
            CollectionPriority.Normal,
            null,
            createdAt.AddDays(-1));
        collectedCollection.AssignDriverAndVehicle(
            driverBruno.Id,
            driverBruno.Name,
            vehicleTruck.Id,
            vehicleTruck.Plate,
            createdAt.AddDays(-1).AddHours(1));
        collectedCollection.MarkInProgress(createdAt.AddDays(-1).AddHours(2));
        collectedCollection.MarkAsCollected(createdAt.AddDays(-1).AddHours(4));

        var cancelledCollection = new Collection(
            Guid.NewGuid(),
            "COL-20260526001",
            customerAcme.Id,
            customerAcme.Name,
            "Fornecedor Oeste",
            "Estrada Oeste, 77",
            "ACME Matriz",
            "Rua Industrial, 45",
            baseDate.AddDays(-2),
            CollectionPriority.High,
            "Remetente solicitou cancelamento.",
            createdAt.AddDays(-2));
        cancelledCollection.Cancel("Remetente indisponível.", createdAt.AddDays(-2).AddHours(2));

        dbContext.AddRange(customerMoura, customerAcme);
        dbContext.AddRange(driverAna, driverBruno);
        dbContext.AddRange(vehicleVan, vehicleTruck);
        dbContext.AddRange(openHighPriorityCollection, inProgressCollection, collectedCollection, cancelledCollection);
        dbContext.SaveChanges();
    }
}
