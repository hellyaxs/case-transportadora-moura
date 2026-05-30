## Why

Hoje o usuário sem sessão válida consegue navegar para telas operacionais do frontend e só descobre que não está autenticado quando a API responde `401 Unauthorized`. Isso cria uma experiência ruim e deixa o estado da aplicação depender de falhas tardias nas chamadas HTTP.

Esta mudança garante que o frontend valide a sessão antes de exibir rotas protegidas e redirecione usuários anônimos para `/login`.

## What Changes

- Adicionar proteção de rotas autenticadas no frontend React/TanStack Router.
- Consultar o estado de autenticação antes de renderizar telas operacionais protegidas.
- Redirecionar usuários sem sessão válida para `/login`, preservando a intenção de destino quando viável.
- Evitar que telas operacionais disparem chamadas de dados protegidos antes da confirmação de autenticação.
- Manter `/login` acessível sem autenticação.

## Capabilities

### New Capabilities

- `frontend-auth-route-protection`: cobre a validação de sessão no frontend, redirecionamento para login e bloqueio de renderização de rotas operacionais para usuários anônimos.

### Modified Capabilities

- `front-back-runtime-integration`: o frontend passa a tratar ausência de sessão antes do carregamento de telas protegidas, evitando depender apenas do `401` retornado pela API para orientar o operador.

## Impact

- Afeta `apps/web/src/routes`, especialmente a árvore de rotas do TanStack Router.
- Afeta `apps/web/src/modules/auth/hooks/auth-provider.tsx` e o fluxo de consulta de sessão atual.
- Pode exigir ajuste em hooks/pages de módulos operacionais para aguardar autenticação antes de buscar dados.
- Não altera contratos públicos da API, banco de dados, OpenAPI ou tipos gerados.
