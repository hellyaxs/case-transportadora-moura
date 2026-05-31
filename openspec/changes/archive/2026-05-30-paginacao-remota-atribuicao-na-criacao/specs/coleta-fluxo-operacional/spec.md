## MODIFIED Requirements

### Requirement: Transições válidas de status

O sistema SHALL controlar transições de status usando enum JSON em inglês: `Open`, `InProgress`, `Collected`, `Cancelled`. Motorista e veículo SHALL ser vinculados no momento da criação da coleta.

#### Scenario: Coleta criada já possui atribuição

- **WHEN** uma coleta é criada com `driverId` e `vehicleId` válidos
- **THEN** a coleta permanece `Open` com atribuição registrada e apta a avançar para `InProgress` conforme regras existentes

#### Scenario: Coleta cancelada é terminal

- **WHEN** uma coleta entra em `Cancelled`
- **THEN** ela não pode retornar para `InProgress` ou `Collected`

### Requirement: Endpoints de fluxo operacional em inglês

A API SHALL expor ações de fluxo em rotas inglesas: `POST /api/collections/{id}/start`, `/complete`, `/cancel`. A atribuição de motorista e veículo SHALL ocorrer exclusivamente na criação via `POST /api/collections`.

#### Scenario: Início via API

- **WHEN** o cliente chama `POST /api/collections/{id}/start` em coleta `Open` com atribuição completa
- **THEN** a coleta avança para `InProgress` conforme regra existente
