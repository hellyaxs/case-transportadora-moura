## ADDED Requirements

### Requirement: Listar coletas operacionais
O sistema SHALL disponibilizar uma listagem operacional de solicitações de coleta com dados suficientes para acompanhamento diário.

#### Scenario: Listagem sem filtros
- **WHEN** um usuário acessa a tela de acompanhamento sem filtros
- **THEN** o sistema exibe as coletas recentes com número, cliente, remetente, destinatário, data prevista, prioridade, status e atribuição operacional

### Requirement: Filtrar coletas por status
O sistema SHALL permitir filtrar a listagem por situação operacional da coleta.

#### Scenario: Filtro por status pendente
- **WHEN** um usuário filtra coletas pelo status `Aberta`
- **THEN** o sistema retorna apenas coletas abertas

### Requirement: Filtrar coletas por cliente
O sistema SHALL permitir filtrar a listagem por cliente.

#### Scenario: Filtro por cliente
- **WHEN** um usuário seleciona um cliente no acompanhamento
- **THEN** o sistema retorna apenas coletas vinculadas ao cliente selecionado

### Requirement: Filtrar coletas por período
O sistema SHALL permitir filtrar a listagem por período de data prevista de retirada.

#### Scenario: Filtro por período
- **WHEN** um usuário informa data inicial e data final
- **THEN** o sistema retorna apenas coletas com data prevista dentro do período informado

### Requirement: Destacar prioridade alta
O sistema SHALL destacar visualmente pedidos com prioridade alta na listagem.

#### Scenario: Coleta de alta prioridade listada
- **WHEN** uma coleta com prioridade alta aparece na listagem
- **THEN** o sistema exibe destaque visual claro para essa coleta

### Requirement: Identificar coletas em atraso
O sistema SHALL permitir identificar rapidamente coletas em atraso.

#### Scenario: Coleta aberta com data vencida
- **WHEN** uma coleta ativa possui data prevista anterior à data atual
- **THEN** o sistema marca a coleta como atrasada na listagem
