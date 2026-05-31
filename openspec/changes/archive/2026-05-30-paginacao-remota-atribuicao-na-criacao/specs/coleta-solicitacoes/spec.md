## MODIFIED Requirements

### Requirement: Criar solicitação de coleta

O sistema SHALL permitir registrar uma solicitação de coleta com número único, cliente, remetente, destinatário, data prevista de retirada, prioridade, observações opcionais, motorista e veículo, expondo contrato HTTP em inglês (`POST /api/collections` com campos `customerId`, `senderName`, `senderAddress`, `recipientName`, `recipientAddress`, `expectedPickupDate`, `priority`, `notes`, `driverId`, `vehicleId`). Na criação bem-sucedida, o sistema SHALL vincular motorista e veículo e registrar `assignedAt`.

#### Scenario: Solicitação criada com dados válidos

- **WHEN** um atendente informa todos os dados obrigatórios da coleta incluindo `driverId` e `vehicleId` válidos
- **THEN** o sistema cria a solicitação com status `Open`, número único de identificação e atribuição operacional já registrada

#### Scenario: Dados obrigatórios ausentes

- **WHEN** a criação da coleta não contém cliente, remetente, destinatário, data prevista de retirada, motorista ou veículo
- **THEN** o sistema rejeita a solicitação e informa os campos obrigatórios pendentes

#### Scenario: Motorista ou veículo inexistente

- **WHEN** a criação informa `driverId` ou `vehicleId` que não existem no catálogo operacional
- **THEN** o sistema rejeita a solicitação com erro de negócio

## ADDED Requirements

### Requirement: Seleção de motorista e veículo no cadastro

O frontend SHALL exibir selects de motorista e veículo no formulário de nova coleta, populados por `GET /api/drivers` e `GET /api/vehicles`, exibindo nome do motorista e placa/descrição do veículo com o `id` correspondente enviado na criação.

#### Scenario: Selects carregados do catálogo

- **WHEN** um operador abre o formulário de nova coleta
- **THEN** o sistema exibe opções de motoristas e veículos retornados pela API

#### Scenario: Criação envia ids selecionados

- **WHEN** um operador seleciona motorista e veículo e submete o formulário
- **THEN** o frontend envia `driverId` e `vehicleId` selecionados em `POST /api/collections`
