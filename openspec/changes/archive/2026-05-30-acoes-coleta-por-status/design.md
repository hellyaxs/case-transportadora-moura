## Context

A tela `CollectionsPage` renderiza quatro botões fixos por item: **Iniciar**, **Concluir**, **Ocorrência** e **Cancelar**, independentemente do `status` retornado pela API. O domínio já restringe transições inválidas no backend, mas a UI não reflete o fluxo operacional.

Não existe hoje endpoint ou ação de exclusão de coleta. O usuário solicitou que coletas `Collected` exibam apenas **Excluir** para remover o registro da listagem.

Matriz de ações desejada:

| Status (`EN`) | Botões visíveis |
|---------------|-----------------|
| `Open` | Iniciar, Ocorrência, Cancelar |
| `InProgress` | Concluir, Ocorrência, Cancelar |
| `Collected` | Excluir |
| `Cancelled` | nenhum botão operacional |

## Goals / Non-Goals

**Goals:**

- Condicionar botões da listagem ao status de cada coleta.
- Permitir exclusão física de coletas `Collected` via API autenticada.
- Confirmar exclusão no frontend antes de chamar a API.
- Cobrir regra de exclusão com testes unitários.

**Non-Goals:**

- Excluir coletas `Open`, `InProgress` ou `Cancelled`.
- Soft delete ou arquivamento; a remoção é física (registro e ocorrências em cascade).
- Alterar regras de transição de status existentes.

## Decisions

### Visibilidade no frontend por status

Extrair renderização dos botões para lógica explícita (função ou componente `CollectionActions`) baseada em `collection.status`.

**Alternativa considerada:** desabilitar botões em vez de ocultar. Rejeitada — ocultar reduz ruído visual e atende ao pedido do produto.

### Exclusão apenas de coletas coletadas

`DELETE /api/collections/{id}` remove fisicamente a coleta somente quando `status === Collected`. Demais status retornam `409 Conflict` com código de negócio (`collection_not_deletable`).

Validação no caso de uso antes de chamar `repository.DeleteAsync`. Não é necessário método de domínio dedicado — a exclusão é operação de limpeza pós-conclusão, não transição de fluxo.

**Alternativa considerada:** permitir exclusão também de `Cancelled`. Fora do escopo solicitado; pode ser adicionada depois.

### Cascade de ocorrências

Reutilizar `DeleteBehavior.Cascade` já configurado em `CollectionConfiguration` para remover `CollectionIncidents` junto com a coleta.

### Confirmação de exclusão

Usar `window.confirm` (mesmo padrão de cancelamento/ocorrência na tela atual) com mensagem clara antes de chamar `DELETE`.

## Risks / Trade-offs

- **[Exclusão irreversível]** → Mitigar com confirmação explícita no frontend e restrição a status `Collected`.
- **[Divergência UI vs API]** → Backend continua sendo fonte de verdade; botões ocultos evitam tentativas, mas a API rejeita exclusão inválida.
- **[Métricas após exclusão]** → Recarregar listagem paginada após delete para atualizar contadores agregados.

## Migration Plan

1. Backend: repositório + caso de uso + endpoint DELETE + testes.
2. Swagger/codegen.
3. Frontend: matriz de botões + service/hook de delete.
4. Atualizar `doc/arquitetura.md`.

Rollback: remover endpoint e revertar condicionais da UI.

## Open Questions

Nenhuma pendente.
