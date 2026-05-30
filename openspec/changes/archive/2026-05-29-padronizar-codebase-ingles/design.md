## Context

O monorepo já usa módulos (`Modules/Collections` após rename), Clean Architecture e codegen OpenAPI. Porém nomes técnicos estão majoritariamente em português (`Coleta`, `Cliente`, `OcorrenciaColeta`, `/api/coletas`). A change deve alinhar código, banco e contratos a inglês sem reescrever specs de produto em português.

## Goals / Non-Goals

**Goals:**

- Estabelecer glossário oficial e aplicá-lo de ponta a ponta no código.
- Renomear backend, frontend, migrations, testes e artefatos gerados.
- Garantir regras permanentes para novos arquivos/código em inglês.
- Manter comportamento de negócio equivalente após rename.

**Non-Goals:**

- Traduzir specs OpenSpec de produto para inglês.
- Renomear conceitos de negócio dentro das specs PT (continuam falando "coleta" no texto).
- Introduzir autenticação ou novas features.

## Decisions

### Glossário oficial de domínio (código e banco)

| Conceito (spec PT) | Código / API / DB (EN) |
|--------------------|-------------------------|
| Coleta | `Collection` |
| Cliente | `Customer` |
| Motorista | `Driver` |
| Veículo | `Vehicle` |
| Ocorrência de coleta | `CollectionIncident` |
| Prioridade Normal/Alta | `Normal` / `High` |
| Status Aberta | `Open` |
| Status EmColeta | `InProgress` |
| Status Coletada | `Collected` |
| Status Cancelada | `Cancelled` |

Propriedades principais:

| PT atual | EN alvo |
|----------|---------|
| `Numero` | `Number` |
| `ClienteId` / `ClienteNome` | `CustomerId` / `CustomerName` |
| `RemetenteNome` / `RemetenteEndereco` | `SenderName` / `SenderAddress` |
| `DestinatarioNome` / `DestinatarioEndereco` | `RecipientName` / `RecipientAddress` |
| `DataPrevistaRetirada` | `ExpectedPickupDate` |
| `Observacoes` | `Notes` |
| `MotoristaId` / `MotoristaNome` | `DriverId` / `DriverName` |
| `VeiculoId` / `VeiculoPlaca` | `VehicleId` / `VehiclePlate` |
| `CriadaEm` / `AtribuidaEm` / `IniciadaEm` / `ColetadaEm` / `CanceladaEm` | `CreatedAt` / `AssignedAt` / `StartedAt` / `CollectedAt` / `CancelledAt` |
| `MotivoCancelamento` | `CancellationReason` |
| `UsuarioResponsavel` | `ResponsibleUser` |
| `RegistradaEm` | `RegisteredAt` |
| `Descricao` | `Description` |

Métodos de domínio:

| PT atual | EN alvo |
|----------|---------|
| `AtribuirMotoristaVeiculo` | `AssignDriverAndVehicle` |
| `MarcarEmColeta` | `MarkInProgress` |
| `MarcarComoColetada` | `MarkAsCollected` |
| `Cancelar` | `Cancel` |
| `RegistrarOcorrencia` | `RegisterIncident` |
| `EstaAtrasada` | `IsOverdue` |

### Módulo e namespaces

- `Modules/Coletas` → `Modules/Collections`
- Namespace `Api.Modules.Coletas.*` → `Api.Modules.Collections.*`
- `ColetasModule` → `CollectionsModule`
- Testes em `__teste__/Modules/Collections/`

Alternativa considerada: manter módulo `Coletas` com classes em inglês. Rejeitada por inconsistência entre pasta, namespace e tipo.

### Rotas REST em inglês

| Rota atual | Rota alvo |
|------------|-----------|
| `/api/coletas` | `/api/collections` |
| `/api/clientes` | `/api/customers` |
| `/api/motoristas` | `/api/drivers` |
| `/api/veiculos` | `/api/vehicles` |
| subpaths `atribuicao`, `iniciar`, `concluir`, `cancelar`, `ocorrencias` | `assignment`, `start`, `complete`, `cancel`, `incidents` |

Alternativa considerada: manter rotas PT com DTOs EN. Rejeitada por inconsistência e pior DX.

### Banco via nova migration

Criar migration `RenameSchemaToEnglish` com `RenameTable`/`RenameColumn` explícitos. Não reescrever migration inicial.

Tabelas:

| PT | EN |
|----|-----|
| `Coletas` | `Collections` |
| `Clientes` | `Customers` |
| `Motoristas` | `Drivers` |
| `Veiculos` | `Vehicles` |
| `OcorrenciasColeta` | `CollectionIncidents` |

Alternativa considerada: drop/recreate schema. Rejeitada por risco de perda de dados em ambientes já migrados.

### Error codes e mensagens técnicas em inglês

Ex.: `cliente_nao_encontrado` → `customer_not_found`. Mensagens de `BusinessRuleException` e ProblemDetails em inglês.

UI React pode continuar exibindo strings PT hardcoded nesta change; services passam a consumir contratos EN.

### Regras Cursor permanentes

Criar `.cursor/rules/english-codebase.mdc` (`alwaysApply: true`) e atualizar regras existentes (`file-naming`, `backend-*`, `openapi-integration`).

Escopo em inglês: `.cs`, `.ts`, `.tsx`, SQL, migrations, Docker/env keys técnicas, OpenAPI property names, testes.

Permitido em português: `openspec/**`, `doc/**`, README de produto, comentários em specs.

### Ordem de execução

1. Regras Cursor + glossário em arquitetura
2. Backend domain/application rename
3. Infrastructure + migration DB
4. Presentation/API routes
5. Regenerar OpenAPI/types
6. Frontend rename + integração
7. Testes + validação

## Risks / Trade-offs

- Breaking change total no contrato HTTP → Mitigar regenerando tipos e atualizando frontend na mesma change.
- Migration rename em PostgreSQL → Mitigar migration testada localmente com `docker compose`.
- Enum JSON muda (`Aberta`→`Open`) → Mitigar documentar mapa na arquitetura; specs PT mantêm termos de negócio.
- Esforço grande de rename → Mitigar execução mecânica por camadas e testes existentes.

## Migration Plan

1. Publicar glossário e regras Cursor.
2. Renomear backend por camadas (git mv + namespaces).
3. Adicionar migration de rename de schema.
4. Atualizar endpoints/DTOs/Swagger.
5. Regenerar `api-types`.
6. Renomear frontend module e calls.
7. Executar `dotnet test`, build web, smoke manual.
8. Atualizar change `autenticacao-jwt-httponily` para usar nomenclatura EN.

Rollback: revert commits; migration `Down` restaura nomes PT se necessário.

## Open Questions

- Textos visíveis da UI devem ser traduzidos nesta change ou ficam PT com contrato EN? **Sugestão:** contrato EN agora; UI PT pode permanecer temporariamente.
- Prefixo de número operacional (`COL-`) permanece ou vira `COL`/`PKP-`? **Sugestão:** manter `COL-` (sigla neutra).
