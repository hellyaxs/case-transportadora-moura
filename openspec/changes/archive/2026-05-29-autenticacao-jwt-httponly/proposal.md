## Why

O case técnico lista autenticação JWT como diferencial e o sistema já exige rastreabilidade de usuário em ocorrências, mas hoje qualquer cliente pode chamar a API sem identidade real e o frontend não possui controle de acesso. Antes de implementar JWT, a API deve estar modularizada (`refatorar-api-modulos-clean-arch`) e padronizada em inglês (`padronizar-codebase-ingles`) para receber o bounded context `Modules/Auth` com Clean Architecture própria.

## What Changes

- Introduzir cadastro persistido de usuários operacionais com credenciais (e-mail e senha com hash).
- Expor endpoints de login, logout e consulta da sessão atual (`/api/auth/*`).
- Emitir JWT assinado pela API e armazená-lo exclusivamente em cookie `HttpOnly`, `Secure` em ambientes HTTPS e `SameSite` compatível com o frontend local.
- **BREAKING**: exigir autenticação em todos os endpoints operacionais existentes de coletas e cadastros.
- **BREAKING**: remover `ResponsibleUser` de `RegisterIncidentRequest`; o responsável passa a vir do usuário autenticado.
- Adicionar tela de login no frontend, guarda de rotas e envio de cookies (`credentials: 'include'`) nas chamadas HTTP.
- Adicionar pacotes NuGet na API: `Microsoft.AspNetCore.Authentication.JwtBearer` (autenticação JWT) e `FluentValidation` (+ integração DI) para validação de requests na Presentation.
- Centralizar validação de requests de auth e revisão dos requests de coletas com validators FluentValidation antes de executar casos de uso.
- Ajustar CORS, variáveis de ambiente, Swagger, seed, testes e documentação de runtime.

## Capabilities

### New Capabilities

- `autenticacao-jwt`: login/logout/sessão atual, emissão e validação de JWT via cookie HttpOnly, proteção global da API e usuários seed para desenvolvimento.

### Modified Capabilities

- `coleta-ocorrencias`: o usuário responsável pela ocorrência deve ser obtido da identidade autenticada, não informado manualmente na requisição.
- `front-back-runtime-integration`: integração local deve suportar cookies autenticados entre frontend e API (CORS com credenciais e chamadas com `credentials: 'include'`).

## Impact

- Backend .NET: nova entidade `Usuario`, migration, serviços de autenticação, `Microsoft.AspNetCore.Authentication.JwtBearer`, validators FluentValidation, middleware/policies JWT, endpoints `Auth`, proteção dos endpoints de coletas e ajuste do caso de uso de ocorrência.
- Frontend React: módulo de autenticação, rota `/login`, proteção das rotas existentes, ajustes em services/hooks e remoção do campo manual de usuário responsável.
- Contratos OpenAPI: novos DTOs/endpoints de auth e alteração de `RegisterIncidentRequest`; regeneração de tipos em `packages/generated/api-types`.
- Infra local: variáveis JWT/CORS/documentação em `.env.example`, `README.md` e `doc/arquitetura.md`.
- Testes: unitários/integrados para login, bloqueio de acesso anônimo e ocorrência com usuário da sessão.
- **Pré-requisito:** aplicar `refatorar-api-modulos-clean-arch` e `padronizar-codebase-ingles` antes desta change; auth deve ser implementada em `Modules/Auth` com nomenclatura EN (`/api/collections`, `RegisterIncidentRequest`, etc.).
- Testes unitários de auth devem ficar em `apps/api/__teste__/Modules/Auth/Application`.
