using Api.Domain.Coletas.Entities;
using Api.Domain.Coletas.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Coletas.Persistence;

public sealed class TransportadoraDbContext(DbContextOptions<TransportadoraDbContext> options) : DbContext(options)
{
    public DbSet<Coleta> Coletas => Set<Coleta>();
    public DbSet<OcorrenciaColeta> OcorrenciasColeta => Set<OcorrenciaColeta>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Motorista> Motoristas => Set<Motorista>();
    public DbSet<Veiculo> Veiculos => Set<Veiculo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureClientes(modelBuilder);
        ConfigureMotoristas(modelBuilder);
        ConfigureVeiculos(modelBuilder);
        ConfigureColetas(modelBuilder);
        ConfigureOcorrencias(modelBuilder);
        Seed(modelBuilder);
    }

    private static void ConfigureClientes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(cliente => cliente.Id);
            entity.Property(cliente => cliente.Id).ValueGeneratedNever();
            entity.Property(cliente => cliente.Nome).HasMaxLength(160).IsRequired();
        });
    }

    private static void ConfigureMotoristas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Motorista>(entity =>
        {
            entity.ToTable("Motoristas");
            entity.HasKey(motorista => motorista.Id);
            entity.Property(motorista => motorista.Id).ValueGeneratedNever();
            entity.Property(motorista => motorista.Nome).HasMaxLength(160).IsRequired();
        });
    }

    private static void ConfigureVeiculos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.ToTable("Veiculos");
            entity.HasKey(veiculo => veiculo.Id);
            entity.Property(veiculo => veiculo.Id).ValueGeneratedNever();
            entity.Property(veiculo => veiculo.Placa).HasMaxLength(16).IsRequired();
            entity.HasIndex(veiculo => veiculo.Placa).IsUnique();
            entity.Property(veiculo => veiculo.Descricao).HasMaxLength(120).IsRequired();
        });
    }

    private static void ConfigureColetas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Coleta>(entity =>
        {
            entity.ToTable("Coletas");
            entity.HasKey(coleta => coleta.Id);
            entity.Property(coleta => coleta.Id).ValueGeneratedNever();
            entity.Property(coleta => coleta.Numero).HasMaxLength(32).IsRequired();
            entity.HasIndex(coleta => coleta.Numero).IsUnique();
            entity.HasIndex(coleta => coleta.Status);
            entity.HasIndex(coleta => coleta.ClienteId);
            entity.HasIndex(coleta => coleta.DataPrevistaRetirada);
            entity.Property(coleta => coleta.ClienteNome).HasMaxLength(160).IsRequired();
            entity.Property(coleta => coleta.RemetenteNome).HasMaxLength(160).IsRequired();
            entity.Property(coleta => coleta.RemetenteEndereco).HasMaxLength(240).IsRequired();
            entity.Property(coleta => coleta.DestinatarioNome).HasMaxLength(160).IsRequired();
            entity.Property(coleta => coleta.DestinatarioEndereco).HasMaxLength(240).IsRequired();
            entity.Property(coleta => coleta.Prioridade).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(coleta => coleta.Status).HasConversion<string>().HasMaxLength(24).IsRequired();
            entity.Property(coleta => coleta.Observacoes).HasMaxLength(500);
            entity.Property(coleta => coleta.MotoristaNome).HasMaxLength(160);
            entity.Property(coleta => coleta.VeiculoPlaca).HasMaxLength(16);
            entity.Property(coleta => coleta.MotivoCancelamento).HasMaxLength(300);
            entity.HasMany(coleta => coleta.Ocorrencias)
                .WithOne()
                .HasForeignKey(ocorrencia => ocorrencia.ColetaId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Navigation(coleta => coleta.Ocorrencias).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }

    private static void ConfigureOcorrencias(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OcorrenciaColeta>(entity =>
        {
            entity.ToTable("OcorrenciasColeta");
            entity.HasKey(ocorrencia => ocorrencia.Id);
            entity.Property(ocorrencia => ocorrencia.Id).ValueGeneratedNever();
            entity.HasIndex(ocorrencia => ocorrencia.ColetaId);
            entity.Property(ocorrencia => ocorrencia.Descricao).HasMaxLength(500).IsRequired();
            entity.Property(ocorrencia => ocorrencia.UsuarioResponsavel).HasMaxLength(120).IsRequired();
            entity.Property(ocorrencia => ocorrencia.RegistradaEm).IsRequired();
        });
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        var baseDate = new DateOnly(2026, 5, 28);
        var createdAt = new DateTimeOffset(2026, 5, 28, 9, 0, 0, TimeSpan.Zero);

        modelBuilder.Entity<Cliente>().HasData(
            new { Id = SeedIds.ClienteMoura, Nome = "Moura Alimentos" },
            new { Id = SeedIds.ClienteAcme, Nome = "ACME Distribuição" });

        modelBuilder.Entity<Motorista>().HasData(
            new { Id = SeedIds.MotoristaAna, Nome = "Ana Souza" },
            new { Id = SeedIds.MotoristaBruno, Nome = "Bruno Lima" });

        modelBuilder.Entity<Veiculo>().HasData(
            new { Id = SeedIds.VeiculoVan, Placa = "ABC1D23", Descricao = "Van urbana" },
            new { Id = SeedIds.VeiculoTruck, Placa = "XYZ9K88", Descricao = "Caminhão baú" });

        modelBuilder.Entity<Coleta>().HasData(
            new
            {
                Id = SeedIds.ColetaAbertaAlta,
                Numero = "COL-20260528001",
                ClienteId = SeedIds.ClienteMoura,
                ClienteNome = "Moura Alimentos",
                RemetenteNome = "Centro de Distribuição Moura",
                RemetenteEndereco = "Rua das Palmeiras, 100",
                DestinatarioNome = "Mercado Central",
                DestinatarioEndereco = "Av. Brasil, 900",
                DataPrevistaRetirada = baseDate.AddDays(-1),
                Prioridade = ColetaPrioridade.Alta,
                Observacoes = "Coleta com janela crítica.",
                Status = ColetaStatus.Aberta,
                MotoristaId = (Guid?)null,
                MotoristaNome = (string?)null,
                VeiculoId = (Guid?)null,
                VeiculoPlaca = (string?)null,
                CriadaEm = createdAt,
                AtribuidaEm = (DateTimeOffset?)null,
                IniciadaEm = (DateTimeOffset?)null,
                ColetadaEm = (DateTimeOffset?)null,
                CanceladaEm = (DateTimeOffset?)null,
                MotivoCancelamento = (string?)null,
            },
            new
            {
                Id = SeedIds.ColetaEmColeta,
                Numero = "COL-20260528002",
                ClienteId = SeedIds.ClienteAcme,
                ClienteNome = "ACME Distribuição",
                RemetenteNome = "Galpão ACME",
                RemetenteEndereco = "Rua Industrial, 45",
                DestinatarioNome = "Loja Norte",
                DestinatarioEndereco = "Rua Norte, 12",
                DataPrevistaRetirada = baseDate,
                Prioridade = ColetaPrioridade.Normal,
                Observacoes = "Conferir volumes no embarque.",
                Status = ColetaStatus.EmColeta,
                MotoristaId = (Guid?)SeedIds.MotoristaAna,
                MotoristaNome = "Ana Souza",
                VeiculoId = (Guid?)SeedIds.VeiculoVan,
                VeiculoPlaca = "ABC1D23",
                CriadaEm = createdAt,
                AtribuidaEm = (DateTimeOffset?)createdAt.AddHours(1),
                IniciadaEm = (DateTimeOffset?)createdAt.AddHours(2),
                ColetadaEm = (DateTimeOffset?)null,
                CanceladaEm = (DateTimeOffset?)null,
                MotivoCancelamento = (string?)null,
            },
            new
            {
                Id = SeedIds.ColetaColetada,
                Numero = "COL-20260527001",
                ClienteId = SeedIds.ClienteMoura,
                ClienteNome = "Moura Alimentos",
                RemetenteNome = "Unidade Sul",
                RemetenteEndereco = "Rua Sul, 300",
                DestinatarioNome = "Cliente Final",
                DestinatarioEndereco = "Av. Atlântica, 88",
                DataPrevistaRetirada = baseDate.AddDays(-1),
                Prioridade = ColetaPrioridade.Normal,
                Observacoes = (string?)null,
                Status = ColetaStatus.Coletada,
                MotoristaId = (Guid?)SeedIds.MotoristaBruno,
                MotoristaNome = "Bruno Lima",
                VeiculoId = (Guid?)SeedIds.VeiculoTruck,
                VeiculoPlaca = "XYZ9K88",
                CriadaEm = createdAt.AddDays(-1),
                AtribuidaEm = (DateTimeOffset?)createdAt.AddDays(-1).AddHours(1),
                IniciadaEm = (DateTimeOffset?)createdAt.AddDays(-1).AddHours(2),
                ColetadaEm = (DateTimeOffset?)createdAt.AddDays(-1).AddHours(4),
                CanceladaEm = (DateTimeOffset?)null,
                MotivoCancelamento = (string?)null,
            },
            new
            {
                Id = SeedIds.ColetaCancelada,
                Numero = "COL-20260526001",
                ClienteId = SeedIds.ClienteAcme,
                ClienteNome = "ACME Distribuição",
                RemetenteNome = "Fornecedor Oeste",
                RemetenteEndereco = "Estrada Oeste, 77",
                DestinatarioNome = "ACME Matriz",
                DestinatarioEndereco = "Rua Industrial, 45",
                DataPrevistaRetirada = baseDate.AddDays(-2),
                Prioridade = ColetaPrioridade.Alta,
                Observacoes = "Remetente solicitou cancelamento.",
                Status = ColetaStatus.Cancelada,
                MotoristaId = (Guid?)null,
                MotoristaNome = (string?)null,
                VeiculoId = (Guid?)null,
                VeiculoPlaca = (string?)null,
                CriadaEm = createdAt.AddDays(-2),
                AtribuidaEm = (DateTimeOffset?)null,
                IniciadaEm = (DateTimeOffset?)null,
                ColetadaEm = (DateTimeOffset?)null,
                CanceladaEm = (DateTimeOffset?)createdAt.AddDays(-2).AddHours(2),
                MotivoCancelamento = (string?)"Remetente indisponível.",
            });

        modelBuilder.Entity<OcorrenciaColeta>().HasData(new
        {
            Id = SeedIds.OcorrenciaEndereco,
            ColetaId = SeedIds.ColetaEmColeta,
            Descricao = "Endereço confirmado por telefone com o remetente.",
            UsuarioResponsavel = "operador.seed",
            RegistradaEm = createdAt.AddHours(2).AddMinutes(15),
        });
    }
}
