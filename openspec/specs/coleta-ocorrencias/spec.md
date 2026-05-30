# Coleta Ocorrências

## Purpose

Definir os recursos de registro, rastreabilidade e consulta de ocorrências de coleta.

## Requirements

### Requirement: Rastreabilidade da ocorrência

O sistema SHALL guardar data, hora e usuário responsável em toda ocorrência registrada, usando a identidade autenticada da sessão atual como origem do usuário responsável.

#### Scenario: Ocorrência criada

- **WHEN** um usuário autenticado registra uma ocorrência em uma coleta
- **THEN** o sistema salva data, hora e identificação do usuário autenticado como responsável

#### Scenario: Usuário responsável ausente

- **WHEN** a requisição de ocorrência não possui sessão autenticada válida
- **THEN** o sistema rejeita o registro antes de persistir a ocorrência

### Requirement: Registrar ocorrência operacional

O sistema SHALL permitir registrar ocorrências via `POST /api/collections/{id}/incidents` com payload `{ description }`, derivando o usuário responsável da sessão autenticada conforme regra vigente de autenticação.

#### Scenario: Ocorrência registrada com dados válidos

- **WHEN** um usuário informa `description` para uma coleta existente
- **THEN** o sistema adiciona o incidente ao histórico da coleta

#### Scenario: Ocorrência sem descrição

- **WHEN** um usuário tenta registrar incidente sem `description`
- **THEN** o sistema rejeita o registro

### Requirement: Consultar ocorrências da coleta

O sistema SHALL permitir consultar as ocorrências de uma solicitação de coleta em ordem cronológica.

#### Scenario: Coleta com múltiplas ocorrências

- **WHEN** um usuário consulta o detalhe de uma coleta com ocorrências registradas
- **THEN** o sistema retorna as ocorrências com descrição, data, hora e usuário responsável

### Requirement: Ocorrências preservadas após cancelamento ou conclusão

O sistema SHALL manter ocorrências vinculadas mesmo quando a coleta estiver cancelada ou coletada.

#### Scenario: Consultar ocorrência de coleta encerrada

- **WHEN** um usuário consulta uma coleta com status `Cancelada` ou `Coletada`
- **THEN** o sistema retorna as ocorrências registradas anteriormente
