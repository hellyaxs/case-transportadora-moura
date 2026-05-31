## ADDED Requirements

### Requirement: Exclusão de coleta coletada

O sistema SHALL expor `DELETE /api/collections/{id}` para remover fisicamente uma coleta somente quando o status for `Collected`. Ocorrências vinculadas SHALL ser removidas em cascade.

#### Scenario: Exclusão de coleta coletada

- **WHEN** o cliente chama `DELETE /api/collections/{id}` para coleta com status `Collected`
- **THEN** o registro e suas ocorrências são removidos e a API retorna `204 No Content`

#### Scenario: Exclusão rejeitada para status ativo

- **WHEN** o cliente tenta excluir coleta com status `Open`, `InProgress` ou `Cancelled`
- **THEN** a API rejeita com erro de negócio indicando que apenas coletas coletadas podem ser excluídas

#### Scenario: Exclusão de coleta inexistente

- **WHEN** o cliente chama `DELETE /api/collections/{id}` para id inexistente
- **THEN** a API retorna `404 Not Found`
