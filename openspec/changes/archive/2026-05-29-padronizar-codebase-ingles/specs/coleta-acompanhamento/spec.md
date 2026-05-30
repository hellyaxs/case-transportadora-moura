## MODIFIED Requirements

### Requirement: Listagem operacional com filtros

O sistema SHALL listar coletas via `GET /api/collections` com filtros JSON `status`, `customerId`, `startDate`, `endDate`.

#### Scenario: Filtro por status

- **WHEN** um operador consulta coletas filtrando por `status=InProgress`
- **THEN** o sistema retorna apenas coletas nesse status

#### Scenario: Filtro por cliente

- **WHEN** um operador filtra por `customerId`
- **THEN** o sistema retorna coletas daquele cliente

### Requirement: Destaque de prioridade alta

O sistema SHALL retornar campo `priority=High` para coletas de alta prioridade, permitindo destaque visual no frontend.

#### Scenario: Listagem contém coleta prioritária

- **WHEN** existem coletas com `priority=High`
- **THEN** a API inclui esse valor no payload de listagem

### Requirement: Identificação de atraso

O sistema SHALL retornar flag booleana `overdue` calculada a partir de `expectedPickupDate` e status ativo.

#### Scenario: Coleta atrasada listada

- **WHEN** uma coleta ativa está além da data prevista
- **THEN** a resposta inclui `overdue=true`
