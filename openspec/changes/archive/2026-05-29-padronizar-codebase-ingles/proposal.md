## Why

O código atual mistura português em classes, métodos, propriedades, tabelas, colunas, rotas HTTP, arquivos e contratos OpenAPI. Isso dificulta manutenção internacional, revisão técnica e consistência com convenções .NET/TypeScript. Precisamos padronizar **todo o código executável e contratos técnicos em inglês**, mantendo português apenas em specs OpenSpec e documentação de produto.

## What Changes

- Adotar glossário único PT→EN para o domínio operacional (Collection, Customer, Driver, Vehicle, CollectionIncident, etc.).
- Renomear módulo `Coletas` → `Collections` e reorganizar pastas/arquivos/namespaces em inglês.
- Renomear entidades, enums, DTOs, use cases, repositórios, endpoints e testes para inglês.
- Renomear tabelas e colunas PostgreSQL via nova migration EF Core (sem editar migration inicial manualmente).
- **BREAKING**: alterar rotas REST (`/api/coletas` → `/api/collections`, etc.) e payloads JSON para nomes em inglês.
- Renomear módulo frontend `coletas` → `collections`, arquivos, hooks, services e tipos gerados.
- Regenerar OpenAPI e `packages/generated/api-types`.
- Criar regras Cursor permanentes exigindo inglês em código (`.cs`, `.ts`, `.tsx`, SQL, migrations, configs técnicas).
- Atualizar `doc/arquitetura.md` com glossário e convenções (documentação pode permanecer em português).
- Traduzir mensagens técnicas em código (exceptions, error codes, logs) para inglês; textos de UI podem permanecer em português nesta change se já estiverem desacoplados.

## Capabilities

### New Capabilities

- `english-codebase-convention`: regras obrigatórias de nomenclatura em inglês para código, banco, contratos e arquivos técnicos.

### Modified Capabilities

- `coleta-solicitacoes`: contratos REST e modelos expostos passam a usar nomenclatura inglesa (`Collection`, `Customer`, `ExpectedPickupDate`, etc.).
- `coleta-fluxo-operacional`: status e ações da API em inglês (`Open`, `InProgress`, `Collected`, `Cancelled`).
- `coleta-ocorrencias`: incidentes com campos e rotas em inglês (`CollectionIncident`, `ResponsibleUser`).
- `coleta-acompanhamento`: filtros e listagem com contrato JSON em inglês.
- `front-back-runtime-integration`: frontend consome novas rotas e tipos gerados em inglês.

## Impact

- Backend .NET: rename amplo + migration de schema + testes renomeados.
- Frontend React: rename de módulo, services, hooks, páginas e integração com tipos gerados.
- Banco PostgreSQL: rename de tabelas/colunas via migration.
- OpenAPI/codegen: breaking change completo.
- Documentação: `doc/arquitetura.md`, regras Cursor; **openspec/specs de produto permanecem em português** (conceitos de negócio), com nota de mapeamento para termos técnicos em inglês.
- Sequenciamento: deve preceder `autenticacao-jwt-httponily` para evitar implementar auth em nomenclatura obsoleta.

## Non-Goals

- Não traduzir `openspec/specs/**`, propostas OpenSpec ou docs de produto para inglês.
- Não traduzir commits históricos ou README completo nesta change (apenas seções técnicas afetadas).
- Não alterar regras de negócio além do necessário para renomeação.
