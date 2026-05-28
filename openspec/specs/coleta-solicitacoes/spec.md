# Coleta Solicitações

## Purpose

Definir os recursos de criação, identificação, priorização e consulta de solicitações de coleta.

## Requirements

### Requirement: Criar solicitação de coleta

O sistema SHALL permitir registrar uma solicitação de coleta com número único, cliente, remetente, destinatário, data prevista de retirada, prioridade e observações opcionais.

#### Scenario: Solicitação criada com dados válidos

- **WHEN** um cliente ou atendente informa todos os dados obrigatórios da coleta
- **THEN** o sistema cria a solicitação com status `Aberta` e número único de identificação

#### Scenario: Dados obrigatórios ausentes

- **WHEN** a criação da coleta não contém cliente, remetente, destinatário ou data prevista de retirada
- **THEN** o sistema rejeita a solicitação e informa os campos obrigatórios pendentes

### Requirement: Identificação única da coleta

O sistema SHALL gerar e preservar um identificador único para cada solicitação de coleta.

#### Scenario: Duas coletas criadas em sequência

- **WHEN** duas solicitações de coleta são criadas
- **THEN** cada solicitação recebe um número de identificação diferente

### Requirement: Prioridade da coleta

O sistema SHALL classificar cada solicitação com uma prioridade operacional explícita.

#### Scenario: Prioridade não informada

- **WHEN** uma solicitação é criada sem prioridade informada
- **THEN** o sistema atribui a prioridade padrão definida pela regra de negócio

#### Scenario: Prioridade alta informada

- **WHEN** uma solicitação é criada com prioridade alta
- **THEN** o sistema preserva essa prioridade para uso na listagem operacional

### Requirement: Consulta de detalhe da coleta

O sistema SHALL permitir consultar os detalhes completos de uma solicitação de coleta.

#### Scenario: Consulta de coleta existente

- **WHEN** um usuário consulta uma coleta existente pelo identificador
- **THEN** o sistema retorna dados da coleta, status atual, atribuição operacional e ocorrências registradas
