# Coleta Fluxo Operacional

## Purpose

Definir o fluxo operacional de atribuição, execução, conclusão e cancelamento de coletas.

## Requirements

### Requirement: Transições válidas de status

O sistema SHALL controlar transições de status usando enum JSON em inglês: `Open`, `InProgress`, `Collected`, `Cancelled`. Motorista e veículo SHALL ser vinculados no momento da criação da coleta.

#### Scenario: Coleta criada já possui atribuição

- **WHEN** uma coleta é criada com `driverId` e `vehicleId` válidos
- **THEN** a coleta permanece `Open` com atribuição registrada e apta a avançar para `InProgress` conforme regras existentes

#### Scenario: Coleta cancelada é terminal

- **WHEN** uma coleta entra em `Cancelled`
- **THEN** ela não pode retornar para `InProgress` ou `Collected`

### Requirement: Conclusão exige motorista e veículo

O sistema SHALL impedir transição para `Collected` sem `driverId` e `vehicleId` vinculados.

#### Scenario: Tentativa de concluir sem atribuição

- **WHEN** uma coleta sem motorista ou veículo tenta avançar para `Collected`
- **THEN** o sistema rejeita a operação com erro de negócio

### Requirement: Endpoints de fluxo operacional em inglês

A API SHALL expor ações de fluxo em rotas inglesas: `POST /api/collections/{id}/start`, `/complete`, `/cancel`. A atribuição de motorista e veículo SHALL ocorrer exclusivamente na criação via `POST /api/collections`.

#### Scenario: Início via API

- **WHEN** o cliente chama `POST /api/collections/{id}/start` em coleta `Open` com atribuição completa
- **THEN** a coleta avança para `InProgress` conforme regra existente

### Requirement: Exclusão de coleta coletada

O sistema SHALL expor `DELETE /api/collections/{id}` para remover fisicamente uma coleta somente quando o status for `Collected`. Ocorrências vinculadas SHALL ser removidas em cascade.

#### Scenario: Exclusão de coleta coletada

- **WHEN** o cliente chama `DELETE /api/collections/{id}` para coleta com status `Collected`
- **THEN** o registro e suas ocorrências são removidos e a API retorna `204 No Content`

#### Scenario: Exclusão rejeitada para status ativo

- **WHEN** o cliente tenta excluir coleta com status `Open`, `InProgress` ou `Cancelled`
- **THEN** a API rejeita com erro de negócio indicando que apenas coletas coletadas podem ser excluídas

#### Scenario: Exclusão de coleta inexistente

- **WHEN** o cliente chama `DELETE /api/collections/{id}` para id inexistente
- **THEN** a API retorna `404 Not Found`

### Requirement: Histórico mínimo de status

O sistema SHALL preservar a data e hora das mudanças operacionais relevantes da coleta.

#### Scenario: Status alterado

- **WHEN** uma coleta é atribuída, iniciada, concluída ou cancelada
- **THEN** o sistema registra a data e hora da alteração correspondente
