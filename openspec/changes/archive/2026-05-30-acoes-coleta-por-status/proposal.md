## Why

A listagem de coletas exibe os mesmos botões de ação para todos os status, o que confunde o operador e permite tentativas inválidas (por exemplo, iniciar uma coleta que já está em execução). Coletas concluídas permanecem na listagem sem uma forma clara de remoção do registro, poluindo o acompanhamento operacional.

## What Changes

- Ocultar o botão **Iniciar** quando a coleta estiver com status `InProgress` (`Em coleta`).
- Exibir **Concluir**, **Ocorrência** e **Cancelar** apenas nos status em que a operação é válida (`Open` e `InProgress`, conforme regras atuais do domínio).
- Quando a coleta estiver `Collected` (`Coletada`), ocultar todos os botões operacionais e exibir apenas **Excluir**.
- Criar endpoint `DELETE /api/collections/{id}` permitindo exclusão física somente de coletas com status `Collected`.
- Adicionar confirmação no frontend antes de excluir um registro coletado.

## Capabilities

### New Capabilities

Nenhuma.

### Modified Capabilities

- `coleta-acompanhamento`: ações da listagem condicionadas ao status da coleta.
- `coleta-fluxo-operacional`: exclusão de registros coletados via API.

## Impact

- **Frontend**: `collections-page.tsx`, hook `use-collections.ts`, `collections.service.ts`.
- **Backend**: novo caso de uso e endpoint `DELETE`, método de repositório, regra de domínio ou validação de status, testes unitários.
- **Contrato OpenAPI**: nova rota DELETE; regeneração de tipos gerados.
- **Documentação**: atualizar `doc/arquitetura.md` com a rota de exclusão.
