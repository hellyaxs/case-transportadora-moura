## Context

O frontend já possui módulo de autenticação em `apps/web/src/modules/auth`, rota pública `/login`, `AuthProvider` e `authService.me()` com `credentials: "include"`. A API protege endpoints operacionais e responde `401 Unauthorized` quando o cookie de sessão está ausente ou inválido.

O problema atual está no momento da validação: a rota `/` renderiza a tela operacional de coletas sem um guard de autenticação. Como consequência, hooks da tela carregam dados protegidos antes de confirmar a sessão e o operador vê erros vindos da API em vez de ser direcionado ao login.

## Goals / Non-Goals

**Goals:**

- Impedir a renderização de rotas operacionais enquanto a sessão não for validada.
- Redirecionar usuários anônimos para `/login` usando o mecanismo de rota do TanStack Router.
- Preservar, quando viável, o destino pretendido para retornar após login.
- Centralizar a regra de proteção em um helper reutilizável do módulo de autenticação.

**Non-Goals:**

- Alterar contratos, endpoints ou formato de resposta da API.
- Persistir JWT em JavaScript, `localStorage` ou `sessionStorage`.
- Implementar autorização por papel/permissão.
- Substituir a validação do backend; a API continua sendo a fronteira de segurança.

## Decisions

1. Usar `beforeLoad` do TanStack Router para proteger rotas operacionais.

   O guard deve executar antes da montagem da página, consultar a sessão atual via `/api/auth/me` e lançar `redirect({ to: "/login" })` quando não houver usuário autenticado. Isso evita que hooks da página iniciem chamadas operacionais protegidas antes da decisão de autenticação.

   Alternativa considerada: redirecionar dentro de componentes React usando `useEffect` e `useAuth`. Essa opção chega tarde demais, porque os componentes filhos podem montar e disparar chamadas à API antes do redirecionamento.

2. Criar helper reutilizável no módulo de autenticação.

   A lógica de proteção deve ficar próxima de `authService`, por exemplo como `requireAuthenticatedUser`, para ser reutilizada por novas rotas protegidas sem duplicar tratamento de erro e redirecionamento. O helper pode retornar o usuário autenticado quando a sessão for válida.

   Alternativa considerada: repetir `authService.me()` em cada arquivo de rota. Funciona para a rota atual, mas espalha a regra de sessão e dificulta manter o mesmo comportamento quando novas telas operacionais forem adicionadas.

3. Manter `/login` como rota pública com redirecionamento para usuários já autenticados.

   A rota de login já consulta `authService.me()` em `beforeLoad` para impedir que usuários autenticados voltem ao formulário. Essa regra deve ser preservada e ajustada apenas se necessário para respeitar o destino pretendido após login.

   Alternativa considerada: proteger todo o root layout. Isso bloquearia também `/login`, exigiria exceções no layout raiz e tornaria o fluxo menos claro.

4. Preservar destino pretendido de forma simples.

   Ao redirecionar para `/login`, o guard deve carregar a rota original em um parâmetro de busca ou mecanismo equivalente do TanStack Router. Após login, a página deve navegar para esse destino se ele for interno e válido; caso contrário, deve usar `/`.

   Alternativa considerada: sempre navegar para `/` após login. É mais simples, mas perde contexto quando o usuário tentou acessar diretamente uma tela protegida.

## Risks / Trade-offs

- Consulta duplicada a `/api/auth/me` entre route guard e `AuthProvider` -> aceitar inicialmente pela simplicidade ou sincronizar o provider após o login/guard se a duplicação ficar perceptível.
- API indisponível durante validação de sessão -> tratar como falha de autenticação para rotas protegidas e manter mensagens compreensíveis no login, sem expor a tela operacional.
- Redirect aberto por parâmetro de retorno mal validado -> aceitar apenas caminhos internos iniciados por `/` e rejeitar URLs absolutas externas.
- Novas rotas podem ser criadas sem o guard -> documentar o helper e incluir tarefas para aplicar o padrão em todas as rotas operacionais existentes.

## Migration Plan

1. Adicionar helper de proteção no módulo de autenticação do frontend.
2. Aplicar o helper no `beforeLoad` das rotas operacionais, começando por `/`.
3. Ajustar `/login` para retornar ao destino pretendido após autenticação bem-sucedida.
4. Validar manualmente os fluxos anônimo, autenticado e sessão expirada.
5. Rodar checagem de tipos do frontend.
