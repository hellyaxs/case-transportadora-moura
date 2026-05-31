## Why

A listagem de coletas carrega todos os registros de uma vez, o que não escala conforme o volume operacional cresce e torna a tela lenta. Além disso, a atribuição de motorista e veículo acontece em um passo separado após a criação, exigindo um botão extra e aumentando o risco de coletas ficarem sem roteirização. Centralizar motorista e veículo no cadastro simplifica o fluxo operacional e reflete melhor o processo real de abertura de serviço.

## What Changes

- Adicionar **paginação remota** na listagem de coletas: a API passa a receber `page` e `pageSize`, retornar apenas a página solicitada e incluir metadados de paginação (`totalCount`, `page`, `pageSize`).
- Manter os filtros existentes (status, cliente, período) aplicados **antes** da paginação no backend.
- Estender a criação de coleta para aceitar `driverId` e `vehicleId`, atribuindo motorista e veículo já na abertura do serviço.
- Expor no formulário de nova coleta selects de motorista e veículo populados pelos endpoints de catálogo operacional (`/api/drivers`, `/api/vehicles`), exibindo nome e placa com o `id` correspondente.
- Remover o botão **Atribuir** da listagem e o fluxo de atribuição posterior no frontend.
- Remover o endpoint `POST /api/collections/{id}/assignment` do contrato público, pois a atribuição passa a ocorrer exclusivamente na criação (**BREAKING**).
- Ajustar métricas do painel para não dependerem apenas dos itens da página atual (endpoint dedicado ou contadores agregados na resposta paginada).

## Capabilities

### New Capabilities

Nenhuma. A mudança estende capacidades já existentes do domínio de coletas.

### Modified Capabilities

- `coleta-acompanhamento`: listagem passa a ser paginada no servidor, com navegação entre páginas no frontend e métricas desacopladas da página corrente.
- `coleta-solicitacoes`: criação de coleta passa a exigir motorista e veículo selecionados no cadastro inicial.
- `coleta-fluxo-operacional`: atribuição deixa de ser uma operação separada pós-criação; motorista e veículo são vinculados no momento da abertura da solicitação.

## Impact

- **Backend .NET**: `ListAsync` e repositório com paginação; novo DTO de resposta paginada; `CreateCollectionRequest`/`CreateCollectionDto` com `DriverId` e `VehicleId`; remoção do endpoint e caso de uso de atribuição separada; testes unitários atualizados.
- **Frontend React**: hook e página de coletas com paginação remota; selects de motorista/veículo no formulário de criação; remoção do botão e handler de atribuição; métricas via resposta agregada da API.
- **Contrato OpenAPI**: resposta paginada em `GET /api/collections`; campos novos em `POST /api/collections`; remoção de `POST /api/collections/{id}/assignment`; regeneração de `packages/generated/api-types`.
- **Documentação**: atualizar `doc/arquitetura.md` se o mapa de rotas ou fluxo operacional mudar.
