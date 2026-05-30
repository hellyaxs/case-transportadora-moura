## Why

A API hoje organiza o código por camadas horizontais (`Domain/Coletas`, `Application/Coletas`, etc.), o que funciona para um único contexto, mas dificulta adicionar novos bounded contexts como autenticação e logging sem misturar responsabilidades. Antes de implementar JWT (`autenticacao-jwt-httponly`), precisamos de uma estrutura modular que permita evoluir cada contexto com suas próprias camadas de Clean Architecture.

## What Changes

- Reorganizar `apps/api/src` em **módulos verticais**, cada um com `Domain`, `Application`, `Infrastructure` e `Presentation`.
- Mover o contexto atual de coletas/transporte para `Modules/Coletas` sem alterar contratos HTTP nem regras de negócio.
- Extrair infraestrutura compartilhada (configuração, persistência base, exceções comuns) para `Shared`.
- Criar esqueleto do módulo `Modules/Auth` com pastas, namespaces e registro DI/endpoints vazio, preparado para a change de autenticação.
- Preparar convenção extensível para futuros módulos (`Logging`, etc.) com método de composição por módulo (`AddXModule`, `MapXEndpoints`).
- Refatorar `Program.cs` para atuar apenas como composition root.
- Reorganizar testes em estrutura **por módulo**, com testes unitários de casos de uso dentro de cada bounded context (ex.: `Modules/Coletas` testa `ColetaUseCases`; `Modules/Auth` recebe pasta de testes preparada).
- Migrar testes existentes de domínio de coletas para a estrutura modular e adicionar cobertura unitária dos use cases de coletas com dependências mockadas.
- Atualizar namespaces, documentação de arquitetura e referências internas afetadas pela movimentação.

## Capabilities

### New Capabilities

- `api-modular-structure`: organização modular da API por bounded context, com camadas internas e composição no host.

### Modified Capabilities

Nenhuma. O comportamento funcional exposto ao frontend permanece o mesmo; esta change é estrutural.

## Impact

- Backend .NET: movimentação de arquivos/namespaces, novos extension methods de composição, `Program.cs` enxuto, esqueleto de `Modules/Auth`.
- Testes: reorganizar `apps/api/__teste__` por módulo; testes de domínio e **casos de uso** colocalizados com cada bounded context; convenção replicável para Auth/Logging.
- Documentação: `doc/arquitetura.md` e regras Cursor de backend.
- Sequenciamento: **deve preceder** `autenticacao-jwt-httponly`, que implementará auth dentro de `Modules/Auth`.
- Sem impacto no frontend, OpenAPI ou contratos públicos nesta change (desde que endpoints e DTOs permaneçam equivalentes).
