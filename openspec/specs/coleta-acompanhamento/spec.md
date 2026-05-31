# Coleta Acompanhamento

## Purpose

Definir os recursos de acompanhamento operacional das solicitações de coleta.

## Requirements

### Requirement: Listagem operacional com filtros

O sistema SHALL listar coletas via `GET /api/collections` com filtros JSON `status`, `customerId`, `startDate`, `endDate` e paginação remota via `page` (base 1, default `1`) e `pageSize` (default `10`, máximo `50`). A resposta SHALL ser um envelope paginado contendo `items`, `page`, `pageSize`, `totalCount` e `totalPages`, aplicando filtros antes da paginação no backend.

#### Scenario: Filtro por status

- **WHEN** um operador consulta coletas filtrando por `status=InProgress`
- **THEN** o sistema retorna apenas coletas nesse status dentro da página solicitada

#### Scenario: Filtro por cliente

- **WHEN** um operador filtra por `customerId`
- **THEN** o sistema retorna coletas daquele cliente dentro da página solicitada

#### Scenario: Paginação remota

- **WHEN** um operador solicita `page=2` e `pageSize=10` com filtros aplicados
- **THEN** o sistema retorna no máximo 10 itens correspondentes à segunda página e metadados `totalCount` e `totalPages` coerentes com o conjunto filtrado

### Requirement: Métricas agregadas na listagem

O sistema SHALL retornar contadores agregados na resposta de `GET /api/collections`, calculados sobre o conjunto filtrado completo (não apenas a página corrente): `openCount`, `inProgressCount`, `overdueCount` e `highPriorityCount`.

#### Scenario: Métricas refletem filtros

- **WHEN** um operador filtra coletas por cliente e solicita uma página específica
- **THEN** os contadores agregados refletem todas as coletas do cliente filtrado, não somente os itens da página exibida

#### Scenario: Navegação entre páginas preserva métricas

- **WHEN** um operador altera apenas `page` mantendo os demais filtros
- **THEN** os contadores agregados permanecem consistentes entre páginas

### Requirement: Navegação de páginas no frontend

O frontend SHALL exibir controles de paginação (anterior/próxima e indicador de página) e recarregar dados do servidor ao mudar página ou filtros, resetando para a página 1 quando filtros forem alterados.

#### Scenario: Mudança de filtro reseta página

- **WHEN** um operador altera o filtro de status estando na página 3
- **THEN** o frontend solicita `page=1` com o novo filtro

#### Scenario: Navegação para próxima página

- **WHEN** existem mais registros além da página atual
- **THEN** o operador pode avançar e o frontend solicita a próxima página ao backend

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

### Requirement: Ações condicionadas ao status na listagem

O frontend SHALL exibir botões de ação por coleta conforme o status retornado pela API:

- `Open`: Iniciar, Ocorrência, Cancelar
- `InProgress`: Concluir, Ocorrência, Cancelar (sem Iniciar)
- `Collected`: Excluir apenas
- `Cancelled`: nenhum botão operacional

#### Scenario: Coleta em execução oculta Iniciar

- **WHEN** uma coleta com status `InProgress` é exibida na listagem
- **THEN** o botão Iniciar não é renderizado

#### Scenario: Coleta aberta exibe Iniciar

- **WHEN** uma coleta com status `Open` é exibida na listagem
- **THEN** o botão Iniciar é exibido e o botão Concluir não é exibido

#### Scenario: Coleta coletada exibe apenas Excluir

- **WHEN** uma coleta com status `Collected` é exibida na listagem
- **THEN** apenas o botão Excluir é exibido

#### Scenario: Coleta cancelada sem ações

- **WHEN** uma coleta com status `Cancelled` é exibida na listagem
- **THEN** nenhum botão operacional é exibido

#### Scenario: Confirmação antes de excluir

- **WHEN** o operador aciona Excluir em uma coleta coletada
- **THEN** o frontend solicita confirmação antes de chamar a API de exclusão
