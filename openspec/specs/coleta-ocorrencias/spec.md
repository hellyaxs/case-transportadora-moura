# Coleta Ocorrências

## Purpose

Definir os recursos de registro, rastreabilidade e consulta de ocorrências de coleta.

## Requirements

### Requirement: Registrar ocorrência operacional

O sistema SHALL permitir registrar ocorrências operacionais vinculadas a uma solicitação de coleta.

#### Scenario: Ocorrência registrada com dados válidos

- **WHEN** um usuário informa descrição da ocorrência para uma coleta existente
- **THEN** o sistema adiciona a ocorrência ao histórico da coleta

#### Scenario: Ocorrência sem descrição

- **WHEN** um usuário tenta registrar ocorrência sem descrição
- **THEN** o sistema rejeita o registro

### Requirement: Rastreabilidade da ocorrência

O sistema SHALL guardar data, hora e usuário responsável em toda ocorrência registrada.

#### Scenario: Ocorrência criada

- **WHEN** uma ocorrência é registrada em uma coleta
- **THEN** o sistema salva data, hora e identificação do usuário responsável

#### Scenario: Usuário responsável ausente

- **WHEN** a origem do usuário responsável não está disponível
- **THEN** o sistema rejeita o registro ou exige identificação controlada do responsável

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
