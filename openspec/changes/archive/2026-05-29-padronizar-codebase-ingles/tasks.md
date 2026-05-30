## 1. Regras e documentação base

- [x] 1.1 Criar `.cursor/rules/english-codebase.mdc` com escopo EN para código e exceção PT para `openspec/**` e `doc/**`
- [x] 1.2 Atualizar regras existentes (`file-naming`, `backend-clean-architecture`, `openapi-integration`) para referenciar nomenclatura em inglês
- [x] 1.3 Adicionar glossário PT→EN e mapa de rotas/contratos em `doc/arquitetura.md`

## 2. Backend — módulo e domínio

- [x] 2.1 Renomear pasta `Modules/Coletas` → `Modules/Collections` e `ColetasModule` → `CollectionsModule`
- [x] 2.2 Renomear entidades, enums e value objects para inglês (`Collection`, `Customer`, `Driver`, `Vehicle`, `CollectionIncident`, status e prioridade)
- [x] 2.3 Renomear métodos de domínio e propriedades conforme glossário (`AssignDriverAndVehicle`, `ExpectedPickupDate`, etc.)
- [x] 2.4 Atualizar namespaces para `Api.Modules.Collections.*` e registrar módulo em `Composition/`

## 3. Backend — application e infrastructure

- [x] 3.1 Renomear use cases, DTOs, interfaces de repositório e handlers para inglês
- [x] 3.2 Renomear EF configurations, `DbSet`s e seed data (`CollectionSeedData`) com mapeamentos EN
- [x] 3.3 Criar migration `RenameSchemaToEnglish` com `RenameTable`/`RenameColumn` para todas as tabelas e colunas afetadas
- [x] 3.4 Traduzir error codes e mensagens técnicas de `BusinessRuleException` para inglês (`customer_not_found`, etc.)

## 4. Backend — presentation e contratos

- [x] 4.1 Renomear endpoints e rotas para `/api/collections`, `/api/customers`, `/api/drivers`, `/api/vehicles` e subpaths em inglês
- [x] 4.2 Atualizar request/response JSON para propriedades em inglês e enums `Open`/`InProgress`/`Collected`/`Cancelled`
- [x] 4.3 Validar Swagger/OpenAPI gerado refletindo contratos em inglês

## 5. Codegen e frontend

- [x] 5.1 Regenerar `packages/generated/api-types` a partir do Swagger atualizado
- [x] 5.2 Renomear módulo frontend `coletas` → `collections` (pastas, arquivos, hooks, services, types)
- [x] 5.3 Atualizar chamadas HTTP, filtros e mapeamentos para novos endpoints e campos JSON em inglês
- [x] 5.4 Ajustar imports/exports e rotas da aplicação web que referenciam o módulo antigo

## 6. Testes e validação

- [x] 6.1 Renomear testes em `__teste__/Modules/Coletas` → `Collections` com nomenclatura EN
- [x] 6.2 Executar `dotnet test` e corrigir falhas relacionadas ao rename
- [x] 6.3 Executar build/lint do frontend e smoke manual (listagem, criação, fluxo operacional)
- [x] 6.4 Atualizar change `autenticacao-jwt-httponily` para referenciar nomenclatura e rotas em inglês
