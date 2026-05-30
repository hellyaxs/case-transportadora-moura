## MODIFIED Requirements

### Requirement: Rastreabilidade da ocorrência

O sistema SHALL guardar data, hora e usuário responsável em toda ocorrência registrada, expondo campos JSON `registeredAt` e `responsibleUser`.

#### Scenario: Ocorrência criada

- **WHEN** uma ocorrência é registrada em uma coleta
- **THEN** o sistema salva `registeredAt` e `responsibleUser` no incidente retornado

#### Scenario: Usuário responsável ausente

- **WHEN** a origem do usuário responsável não está disponível
- **THEN** o sistema rejeita o registro ou exige identificação controlada do responsável

### Requirement: Registrar ocorrência operacional

O sistema SHALL permitir registrar ocorrências via `POST /api/collections/{id}/incidents` com payload `{ description, responsibleUser? }` conforme regra vigente de autenticação.

#### Scenario: Ocorrência registrada com dados válidos

- **WHEN** um usuário informa `description` para uma coleta existente
- **THEN** o sistema adiciona o incidente ao histórico da coleta

#### Scenario: Ocorrência sem descrição

- **WHEN** um usuário tenta registrar incidente sem `description`
- **THEN** o sistema rejeita o registro
