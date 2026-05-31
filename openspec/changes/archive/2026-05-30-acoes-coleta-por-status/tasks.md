## 1. Backend — exclusão de coletas coletadas

- [x] 1.1 Adicionar `DeleteAsync` em `ICollectionRepository` e `EfCollectionRepository`
- [x] 1.2 Implementar `DeleteAsync` em `CollectionUseCases` com validação de status `Collected`
- [x] 1.3 Expor `DELETE /api/collections/{id}` em `CollectionsEndpoints` retornando `204 No Content`
- [x] 1.4 Adicionar testes unitários para exclusão permitida, status inválido e coleta inexistente

## 2. Contrato OpenAPI e tipos gerados

- [x] 2.1 Atualizar `swagger.json` e regenerar tipos com `api-swagger:codegen`
- [x] 2.2 Verificar método delete no client gerado ou service manual do frontend

## 3. Frontend — ações por status

- [x] 3.1 Criar lógica/componente de ações condicionadas ao `status` da coleta
- [x] 3.2 Ocultar **Iniciar** em `InProgress` e **Concluir** em `Open`
- [x] 3.3 Exibir apenas **Excluir** em `Collected`, com confirmação prévia
- [x] 3.4 Ocultar todos os botões em `Cancelled`
- [x] 3.5 Adicionar `delete` em `collections.service.ts` e `use-collections.ts`

## 4. Validação e documentação

- [x] 4.1 Executar `dotnet test apps/api/__teste__/Api.Test.csproj`
- [x] 4.2 Executar `pnpm check-types` (ou build web)
- [x] 4.3 Atualizar `doc/arquitetura.md` com rota `DELETE /api/collections/{id}`
