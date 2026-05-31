## 1. Backend — paginação e métricas

- [x] 1.1 Criar DTOs `PaginatedCollectionResponseDto` e `CollectionListMetricsDto` em `Application/Dtos`
- [x] 1.2 Estender `ICollectionRepository` com `CountAsync` e `ListPagedAsync` (filtros + `page`/`pageSize`, ordenação por `CreatedAt` desc)
- [x] 1.3 Implementar queries paginadas e agregadas em `EfCollectionRepository`
- [x] 1.4 Atualizar `CollectionUseCases.ListAsync` para retornar envelope paginado com métricas
- [x] 1.5 Expor query params `page` e `pageSize` em `GET /api/collections` e atualizar documentação Swagger

## 2. Backend — atribuição na criação

- [x] 2.1 Adicionar `DriverId` e `VehicleId` em `CreateCollectionDto`, `CreateCollectionRequest` e validação de entrada
- [x] 2.2 Atualizar `CreateAsync` para validar driver/vehicle e chamar `AssignDriverAndVehicle` na mesma transação
- [x] 2.3 Remover endpoint `POST /api/collections/{id}/assignment`, `AssignCollectionRequest` e método `AssignAsync`
- [x] 2.4 Atualizar testes em `CollectionUseCasesTests` (criação com atribuição, paginação, remoção de assign)

## 3. Contrato OpenAPI e tipos gerados

- [x] 3.1 Regenerar Swagger com `pnpm nx run api:swagger`
- [x] 3.2 Regenerar tipos TypeScript com `pnpm nx run api-swagger:codegen`
- [x] 3.3 Verificar remoção de `AssignCollectionRequest` e presença do envelope paginado nos tipos gerados

## 4. Frontend — listagem paginada

- [x] 4.1 Atualizar `collections.service.ts` para enviar `page`/`pageSize` e consumir resposta paginada
- [x] 4.2 Refatorar `use-collections.ts` com estado de paginação, métricas da API e reset de página ao mudar filtros
- [x] 4.3 Adicionar controles de navegação (anterior/próxima, indicador de página) em `collections-page.tsx`
- [x] 4.4 Remover cálculo local de métricas a partir do array de coletas da página

## 5. Frontend — formulário com motorista e veículo

- [x] 5.1 Adicionar selects de motorista (`name`) e veículo (`plate`/`description`) no formulário de nova coleta
- [x] 5.2 Enviar `driverId` e `vehicleId` no `POST /api/collections`
- [x] 5.3 Remover botão **Atribuir**, `handleAssign` e método `assign` do hook/service

## 6. Validação e documentação

- [x] 6.1 Executar `dotnet test apps/api/__teste__/Api.Test.csproj`
- [x] 6.2 Executar `pnpm check-types` e validar fluxo manual (criar → paginar → iniciar/concluir)
- [x] 6.3 Atualizar `doc/arquitetura.md` (rotas, fluxo de criação com atribuição, paginação remota)
