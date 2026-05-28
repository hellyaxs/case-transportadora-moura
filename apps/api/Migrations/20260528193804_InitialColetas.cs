using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialColetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coletas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Numero = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteNome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    RemetenteNome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    RemetenteEndereco = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    DestinatarioNome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    DestinatarioEndereco = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    DataPrevistaRetirada = table.Column<DateOnly>(type: "date", nullable: false),
                    Prioridade = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    MotoristaId = table.Column<Guid>(type: "uuid", nullable: true),
                    MotoristaNome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    VeiculoId = table.Column<Guid>(type: "uuid", nullable: true),
                    VeiculoPlaca = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    CriadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtribuidaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IniciadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ColetadaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CanceladaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    MotivoCancelamento = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coletas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motoristas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motoristas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Placa = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OcorrenciasColeta",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ColetaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UsuarioResponsavel = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    RegistradaEm = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OcorrenciasColeta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OcorrenciasColeta_Coletas_ColetaId",
                        column: x => x.ColetaId,
                        principalTable: "Coletas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Moura Alimentos" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "ACME Distribuição" }
                });

            migrationBuilder.InsertData(
                table: "Coletas",
                columns: new[] { "Id", "AtribuidaEm", "CanceladaEm", "ClienteId", "ClienteNome", "ColetadaEm", "CriadaEm", "DataPrevistaRetirada", "DestinatarioEndereco", "DestinatarioNome", "IniciadaEm", "MotivoCancelamento", "MotoristaId", "MotoristaNome", "Numero", "Observacoes", "Prioridade", "RemetenteEndereco", "RemetenteNome", "Status", "VeiculoId", "VeiculoPlaca" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, null, new Guid("11111111-1111-1111-1111-111111111111"), "Moura Alimentos", null, new DateTimeOffset(new DateTime(2026, 5, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 5, 27), "Av. Brasil, 900", "Mercado Central", null, null, null, null, "COL-20260528001", "Coleta com janela crítica.", "Alta", "Rua das Palmeiras, 100", "Centro de Distribuição Moura", "Aberta", null, null },
                    { new Guid("88888888-8888-8888-8888-888888888888"), new DateTimeOffset(new DateTime(2026, 5, 28, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("22222222-2222-2222-2222-222222222222"), "ACME Distribuição", null, new DateTimeOffset(new DateTime(2026, 5, 28, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 5, 28), "Rua Norte, 12", "Loja Norte", new DateTimeOffset(new DateTime(2026, 5, 28, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("33333333-3333-3333-3333-333333333333"), "Ana Souza", "COL-20260528002", "Conferir volumes no embarque.", "Normal", "Rua Industrial, 45", "Galpão ACME", "EmColeta", new Guid("55555555-5555-5555-5555-555555555555"), "ABC1D23" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), new DateTimeOffset(new DateTime(2026, 5, 27, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("11111111-1111-1111-1111-111111111111"), "Moura Alimentos", new DateTimeOffset(new DateTime(2026, 5, 27, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 5, 27, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 5, 27), "Av. Atlântica, 88", "Cliente Final", new DateTimeOffset(new DateTime(2026, 5, 27, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, new Guid("44444444-4444-4444-4444-444444444444"), "Bruno Lima", "COL-20260527001", null, "Normal", "Rua Sul, 300", "Unidade Sul", "Coletada", new Guid("66666666-6666-6666-6666-666666666666"), "XYZ9K88" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new DateTimeOffset(new DateTime(2026, 5, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("22222222-2222-2222-2222-222222222222"), "ACME Distribuição", null, new DateTimeOffset(new DateTime(2026, 5, 26, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new DateOnly(2026, 5, 26), "Rua Industrial, 45", "ACME Matriz", null, "Remetente indisponível.", null, null, "COL-20260526001", "Remetente solicitou cancelamento.", "Alta", "Estrada Oeste, 77", "Fornecedor Oeste", "Cancelada", null, null }
                });

            migrationBuilder.InsertData(
                table: "Motoristas",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Ana Souza" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Bruno Lima" }
                });

            migrationBuilder.InsertData(
                table: "Veiculos",
                columns: new[] { "Id", "Descricao", "Placa" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Van urbana", "ABC1D23" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), "Caminhão baú", "XYZ9K88" }
                });

            migrationBuilder.InsertData(
                table: "OcorrenciasColeta",
                columns: new[] { "Id", "ColetaId", "Descricao", "RegistradaEm", "UsuarioResponsavel" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("88888888-8888-8888-8888-888888888888"), "Endereço confirmado por telefone com o remetente.", new DateTimeOffset(new DateTime(2026, 5, 28, 11, 15, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "operador.seed" });

            migrationBuilder.CreateIndex(
                name: "IX_Coletas_ClienteId",
                table: "Coletas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Coletas_DataPrevistaRetirada",
                table: "Coletas",
                column: "DataPrevistaRetirada");

            migrationBuilder.CreateIndex(
                name: "IX_Coletas_Numero",
                table: "Coletas",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coletas_Status",
                table: "Coletas",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OcorrenciasColeta_ColetaId",
                table: "OcorrenciasColeta",
                column: "ColetaId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_Placa",
                table: "Veiculos",
                column: "Placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Motoristas");

            migrationBuilder.DropTable(
                name: "OcorrenciasColeta");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Coletas");
        }
    }
}
