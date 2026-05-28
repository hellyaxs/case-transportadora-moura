# Coleta Fluxo Operacional

## Purpose

Definir o fluxo operacional de atribuição, execução, conclusão e cancelamento de coletas.

## Requirements

### Requirement: Atribuir motorista e veículo

O sistema SHALL permitir atribuir motorista e veículo a uma solicitação de coleta ativa antes da execução operacional.

#### Scenario: Atribuição em coleta aberta

- **WHEN** uma coleta com status `Aberta` recebe motorista e veículo válidos
- **THEN** o sistema grava a atribuição operacional da coleta

#### Scenario: Atribuição em coleta cancelada

- **WHEN** um usuário tenta atribuir motorista ou veículo a uma coleta com status `Cancelada`
- **THEN** o sistema rejeita a operação porque a coleta não pode voltar ao fluxo ativo

### Requirement: Iniciar execução da coleta

O sistema SHALL permitir avançar uma coleta ativa para `EmColeta` somente quando houver motorista e veículo atribuídos.

#### Scenario: Iniciar coleta com atribuição completa

- **WHEN** uma coleta `Aberta` possui motorista e veículo atribuídos
- **THEN** o sistema permite alterar o status para `EmColeta`

#### Scenario: Iniciar coleta sem atribuição completa

- **WHEN** uma coleta não possui motorista ou veículo vinculado
- **THEN** o sistema rejeita a alteração para `EmColeta`

### Requirement: Concluir coleta

O sistema SHALL permitir marcar uma coleta como `Coletada` somente quando houver motorista e veículo vinculados.

#### Scenario: Concluir coleta em execução

- **WHEN** uma coleta `EmColeta` possui motorista e veículo vinculados
- **THEN** o sistema altera o status para `Coletada`

#### Scenario: Concluir coleta sem motorista ou veículo

- **WHEN** um usuário tenta marcar uma coleta como `Coletada` sem motorista ou veículo vinculado
- **THEN** o sistema rejeita a conclusão

### Requirement: Cancelamento terminal

O sistema SHALL tratar `Cancelada` como status terminal, sem retorno para `Aberta`, `EmColeta` ou `Coletada`.

#### Scenario: Cancelar coleta ativa

- **WHEN** uma coleta com status `Aberta` ou `EmColeta` é cancelada
- **THEN** o sistema altera o status para `Cancelada` e mantém o registro da coleta

#### Scenario: Reativar coleta cancelada

- **WHEN** um usuário tenta alterar uma coleta `Cancelada` para `EmColeta` ou `Coletada`
- **THEN** o sistema rejeita a transição

### Requirement: Histórico mínimo de status

O sistema SHALL preservar a data e hora das mudanças operacionais relevantes da coleta.

#### Scenario: Status alterado

- **WHEN** uma coleta é atribuída, iniciada, concluída ou cancelada
- **THEN** o sistema registra a data e hora da alteração correspondente
