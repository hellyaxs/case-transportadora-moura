## Context

A aplicação já entrega gestão de coletas com API .NET, frontend React e PostgreSQL. O case técnico (`case_backend.pdf`) cita autenticação JWT como diferencial e o domínio de ocorrências exige usuário responsável rastreável. Hoje a API aceita chamadas anônimas e o frontend envia `ResponsibleUser` manualmente no body — solução provisória documentada na change anterior de coletas.

O monorepo usa CORS por origem (`FRONTEND_ORIGIN`), fetch direto do browser para a API e cookies ainda não participam do fluxo. A autenticação precisa funcionar localmente (web em `3001`, API em `5086`) e no Docker Compose.

**Pré-requisitos:** esta change assume a estrutura modular da API (`Modules/Auth`, `Modules/Collections`, `Shared`) e nomenclatura em inglês criadas em `refatorar-api-modulos-clean-arch` e `padronizar-codebase-ingles`.

## Goals / Non-Goals

**Goals:**

- Garantir que todo acesso operacional (API e telas internas) exija sessão autenticada válida.
- Emitir JWT assinado no backend e persistir sessão apenas em cookie `HttpOnly`.
- Permitir login/logout e consulta do usuário autenticado.
- Popular usuário responsável de ocorrências a partir das claims da sessão.
- Manter Clean Architecture: domínio de identidade separado de coletas; Presentation apenas orquestra HTTP/cookies.
- Cobrir fluxos críticos com testes e documentação de execução local.

**Non-Goals:**

- Não implementar cadastro público, recuperação de senha, MFA ou OAuth externo.
- Não modelar RBAC/perfis granulares além de usuário operacional autenticado.
- Não migrar para refresh token rotativo ou filas de revogação distribuída nesta change.
- Não proteger Swagger UI além do necessário para desenvolvimento local (pode permanecer acessível em Development).

## Decisions

### JWT em cookie HttpOnly (sem token no frontend)

A API gera JWT curto (ex.: 8h) e grava em cookie `HttpOnly` (`auth_token`). O frontend nunca lê nem armazena o token em `localStorage`/`sessionStorage`.

Alternativa considerada: Bearer token no header Authorization gerenciado pelo frontend. Rejeitada por aumentar risco de XSS e exigir lógica manual de armazenamento no browser.

### Validação JWT com `Microsoft.AspNetCore.Authentication.JwtBearer`

Usar o pacote `Microsoft.AspNetCore.Authentication.JwtBearer` para configurar autenticação JWT no ASP.NET Core 8, com evento de leitura do token a partir do cookie `HttpOnly` antes da validação padrão de assinatura, issuer, audience e expiração. Endpoints protegidos usam `RequireAuthorization()`; login/logout permanecem anônimos.

Alternativa considerada: middleware customizado sem integração com `ClaimsPrincipal`. Rejeitada por duplicar comportamento padrão e dificultar testes.

### Validação de requests com FluentValidation

Adicionar `FluentValidation` e `FluentValidation.DependencyInjectionExtensions` em `apps/api/Api.csproj`. Criar validators na Presentation para requests de auth (login) e revisar validators dos requests de collections afetados por esta change (ex.: ocorrência sem `ResponsibleUser`). Endpoints devem rejeitar payloads inválidos com resposta padronizada antes de chamar Application.

Alternativa considerada: validação manual inline nos endpoints. Rejeitada por duplicar regras, dificultar testes e inconsistente com evolução do contrato HTTP.

### Entidade `Usuario` com senha hash (BCrypt ou PBKDF2)

Persistir usuários operacionais com e-mail único e hash de senha. Seed inicial cria ao menos um operador demo para desenvolvimento e testes.

Alternativa considerada: usuários hardcoded em configuração. Rejeitada por não demonstrar modelagem de dados nem suportar múltiplos operadores.

### Ocorrências usam identidade autenticada

`RegisterIncidentRequest` deixa de aceitar `ResponsibleUser`. O use case recebe o identificador/nome do usuário via `IHttpContextAccessor` ou abstração `ICurrentUser` na Application.

Alternativa considerada: manter campo opcional com fallback. Rejeitada por permitir falsificação de responsável.

### CORS com credenciais

Política CORS deve usar `AllowCredentials()` e origem explícita (não `*`). Frontend passa `credentials: 'include'` em todas as chamadas autenticadas.

Alternativa considerada: proxy same-origin no Vite. Pode ser complemento futuro, mas não substitui cookies cross-origin no cenário atual.

### Frontend com rota `/login` e guarda de sessão

TanStack Router redireciona rotas protegidas para `/login` quando `/api/auth/me` retorna 401. Após login bem-sucedido, redireciona para a home operacional.

Alternativa considerada: modal de login sobre a tela atual. Rejeitada por complicar estado global e deep links.

## Risks / Trade-offs

- Cookie cross-origin em dev (`3001` → `5086`) → Mitigar com `SameSite=None; Secure` apenas quando necessário ou documentar proxy; em dev local HTTP usar `SameSite=Lax` se front e API compartilharem estratégia documentada.
- JWT sem refresh token → Mitigar com expiração moderada e logout explícito limpando cookie.
- **BREAKING** para integrações existentes → Mitigar documentando login obrigatório, seed de usuário e exemplos curl/Postman com cookie jar.
- Swagger em Development ainda expõe endpoints → Mitigar indicando uso de autenticação via cookie ou futura integração Swashbuckle + cookie auth.

## Migration Plan

1. Adicionar pacotes NuGet (`Microsoft.AspNetCore.Authentication.JwtBearer`, `FluentValidation`, `FluentValidation.DependencyInjectionExtensions`) e configurar DI/validators.
2. Adicionar entidade/migration/seed de usuários e configuração JWT.
3. Implementar endpoints de auth e autenticação JWT via cookie HttpOnly.
4. Proteger endpoints existentes e ajustar ocorrências para `ICurrentUser`.
5. Regenerar OpenAPI/tipos e adaptar frontend (login, guards, fetch credentials).
6. Atualizar `.env.example`, README, arquitetura e testes.
7. Validar fluxo manual: login → operar coletas → registrar ocorrência → logout → acesso bloqueado.

Rollback: reverter proteção global e restaurar contrato anterior de ocorrência apenas se necessário; em ambiente dev, rollback é revert de migration + commits.

## Open Questions

- Nome do cookie e tempo de expiração preferidos para demo (sugestão: `auth_token`, 8 horas)?
- Swagger em Development deve exigir login ou permanece aberto com endpoints protegidos apenas na execução?
