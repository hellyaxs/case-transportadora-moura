## Context

A tela de coletas (`CollectionsPage`) carrega todas as coletas via `GET /api/collections` sem parâmetros de paginação e renderiza a lista completa no cliente. As métricas do painel (abertas, em coleta, atrasadas, alta prioridade) são calculadas em memória a partir desse array, o que deixa de ser confiável quando apenas uma página é exibida.

A criação de coleta (`POST /api/collections`) aceita apenas dados da solicitação. Motorista e veículo são vinculados depois via `POST /api/collections/{id}/assignment`, acionado pelo botão **Atribuir** na listagem — que hoje usa o primeiro motorista e veículo do catálogo sem escolha explícita do operador.

Os catálogos operacionais já existem:
- `GET /api/drivers` → `OptionDto { id, name }`
- `GET /api/vehicles` → `VehicleOptionDto { id, plate, description }`

O domínio já possui `AssignDriverAndVehicle` na entidade `Collection`; a mudança reutiliza esse método dentro do caso de uso de criação.

## Goals / Non-Goals

**Goals:**

- Paginar a listagem no servidor com filtros existentes preservados.
- Exigir motorista e veículo no cadastro inicial da coleta.
- Popular selects de motorista e veículo a partir dos catálogos da API.
- Remover o fluxo de atribuição posterior no frontend e o endpoint público de assignment.
- Manter métricas operacionais corretas independentemente da página exibida.
- Regenerar contrato OpenAPI e tipos TypeScript.

**Non-Goals:**

- Paginação ou busca nos catálogos de motoristas/veículos (listas pequenas, carregamento único).
- Alterar regras de transição de status (`start`, `complete`, `cancel`) além da origem da atribuição.
- Implementar ordenação customizável ou cursor-based pagination.
- Permitir reatribuição de motorista/veículo após a criação.

## Decisions

### Resposta paginada em `GET /api/collections`

Introduzir query params `page` (base 1, default `1`) e `pageSize` (default `10`, máximo `50`).

A resposta deixa de ser `CollectionSummaryDto[]` e passa a ser um envelope:

```json
{
  "items": [ /* CollectionSummaryDto */ ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 87,
  "totalPages": 9,
  "metrics": {
    "openCount": 12,
    "inProgressCount": 5,
    "overdueCount": 3,
    "highPriorityCount": 2
  }
}
```

**Alternativa considerada:** endpoint separado `GET /api/collections/metrics`. Rejeitada para evitar roundtrip extra na tela principal; métricas usam os mesmos filtros da listagem, sem paginação.

**Alternativa considerada:** paginação apenas no frontend. Rejeitada — não escala e contradiz o requisito de paginação remota.

### Ordenação padrão

Listagem ordenada por `CreatedAt` descendente (mais recentes primeiro), aplicada no repositório antes do `Skip/Take`.

**Alternativa considerada:** ordenar por `ExpectedPickupDate`. Mantida como decisão futura; por ora prioriza o padrão operacional de ver solicitações recentes.

### Atribuição na criação

Estender `CreateCollectionRequest` / `CreateCollectionDto` com `driverId` e `vehicleId` obrigatórios.

No `CreateAsync`:
1. Validar existência de customer, driver e vehicle no repositório.
2. Instanciar `Collection` com status `Open`.
3. Chamar `AssignDriverAndVehicle` imediatamente, registrando `AssignedAt`.
4. Persistir em uma única transação.

**Alternativa considerada:** manter endpoint de assignment para reatribuição. Rejeitada conforme escopo — atribuição exclusiva na criação simplifica o fluxo.

### Remoção do endpoint de assignment

Remover `POST /api/collections/{id}/assignment`, `AssignCollectionRequest`, `AssignAsync` no caso de uso e referências no frontend/service gerado.

**Breaking change** aceito: único consumidor é o frontend interno, que será atualizado na mesma change.

Manter `AssignDriverAndVehicle` no domínio — usado internamente na criação.

### Frontend: paginação e formulário

- `useCollections`: estado `page` e `pageSize`; recarregar ao mudar filtros (resetando para página 1).
- Controles Anterior/Próxima e indicador "Página X de Y".
- Formulário de nova coleta: dois `<select>` para motorista (nome) e veículo (placa + descrição).
- Remover `handleAssign`, botão **Atribuir** e método `assign` do hook/service.

### Contrato OpenAPI

Seguir fluxo existente:
```bash
pnpm nx run api:swagger
pnpm nx run api-swagger:codegen
```

## Risks / Trade-offs

- **[Breaking] Remoção do endpoint de assignment** → Mitigar atualizando frontend e regenerando tipos na mesma entrega; documentar no changelog da change.
- **Métricas com filtros amplos podem ser mais pesadas** → Mitigar com queries agregadas (`COUNT` com mesmos filtros) em vez de carregar todos os registros.
- **Motorista/veículo obrigatórios na criação** → Mitigar validação clara no backend (`400`) e selects obrigatórios no formulário; seed deve garantir catálogos não vazios em dev.
- **Impossibilidade de reatribuir após criação** → Aceito no escopo atual; reatribuição futura exigiria nova change com regra de negócio explícita.

## Migration Plan

1. Backend: DTO paginado, repositório com `CountAsync` + `ListPagedAsync`, métricas agregadas, criação com atribuição, remoção do endpoint de assignment.
2. Testes: atualizar `CollectionUseCasesTests` e adicionar cenários de paginação e criação com atribuição.
3. Swagger + codegen.
4. Frontend: hook, página, service; remover atribuição posterior.
5. Validar fluxo manual: criar coleta com selects → listar paginado → iniciar/concluir sem botão Atribuir.
6. Atualizar `doc/arquitetura.md` (rotas e fluxo operacional).

Rollback: reverter commits; sem migration de schema (apenas contrato e lógica).

## Open Questions

Nenhuma pendente — escopo definido pelo produto nesta proposta.
