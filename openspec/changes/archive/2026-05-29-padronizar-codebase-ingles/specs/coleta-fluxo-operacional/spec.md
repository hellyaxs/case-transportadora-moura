## MODIFIED Requirements

### Requirement: Transições válidas de status

O sistema SHALL controlar transições de status usando enum JSON em inglês: `Open`, `InProgress`, `Collected`, `Cancelled`.

#### Scenario: Coleta aberta recebe atribuição

- **WHEN** motorista e veículo são vinculados a uma coleta `Open`
- **THEN** a coleta permanece apta a avançar para `InProgress` ou `Collected` conforme regras existentes

#### Scenario: Coleta cancelada é terminal

- **WHEN** uma coleta entra em `Cancelled`
- **THEN** ela não pode retornar para `InProgress` ou `Collected`

### Requirement: Conclusão exige motorista e veículo

O sistema SHALL impedir transição para `Collected` sem `driverId` e `vehicleId` vinculados.

#### Scenario: Tentativa de concluir sem atribuição

- **WHEN** uma coleta sem motorista ou veículo tenta avançar para `Collected`
- **THEN** o sistema rejeita a operação com erro de negócio

### Requirement: Endpoints de fluxo operacional em inglês

A API SHALL expor ações de fluxo em rotas inglesas: `POST /api/collections/{id}/assignment`, `/start`, `/complete`, `/cancel`.

#### Scenario: Atribuição via API

- **WHEN** o cliente chama `POST /api/collections/{id}/assignment` com `driverId` e `vehicleId`
- **THEN** a coleta é atualizada conforme regra de atribuição existente
