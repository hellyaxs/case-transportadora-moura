## 0. Pré-requisitos

- [x] 0.1 Concluir a change `refatorar-api-modulos-clean-arch` antes de iniciar esta implementação.
- [x] 0.2 Concluir a change `padronizar-codebase-ingles` (rotas `/api/collections`, requests EN) antes de iniciar esta implementação.

## 1. Dependências e Validação na API

- [x] 1.1 Adicionar `Microsoft.AspNetCore.Authentication.JwtBearer` em `apps/api/Api.csproj`.
- [x] 1.2 Adicionar `FluentValidation` e `FluentValidation.DependencyInjectionExtensions` em `apps/api/Api.csproj`.
- [x] 1.3 Registrar validators FluentValidation no `Program.cs` e helper de validação reutilizável para Minimal APIs.
- [x] 1.4 Criar validators de auth (login) e revisar validators dos requests de coletas alterados nesta change.

## 2. Domínio e Persistência de Usuários (Modules/Auth)

- [x] 2.1 Criar entidade `Usuario` em `Modules/Auth/Domain` com e-mail, nome e senha hash.
- [x] 2.2 Mapear `Usuario` no DbContext compartilhado com configuração em `Modules/Auth/Infrastructure`.
- [x] 2.3 Criar migration `AddUsuariosAuth` e seed de usuário operacional demo.
- [x] 2.4 Implementar repositório/contrato de usuários em `Modules/Auth/Application` e `Infrastructure`.

## 3. Autenticação JWT e Cookies na API (Modules/Auth)

- [x] 3.1 Adicionar configuração JWT (`JWT_SECRET`, issuer, audience, expiração) em appsettings/.env.example.
- [x] 3.2 Implementar serviço de autenticação (validação de senha, emissão de token, claims).
- [x] 3.3 Configurar `AddAuthentication().AddJwtBearer()` lendo JWT do cookie `HttpOnly`.
- [x] 3.4 Criar endpoints auth em `Modules/Auth/Presentation`: `POST /api/auth/login`, `POST /api/auth/logout` e `GET /api/auth/me`.
- [x] 3.5 Ajustar CORS para `AllowCredentials()` com origem explícita do frontend.
- [x] 3.6 Proteger endpoints de coletas/cadastros com `RequireAuthorization()`.
- [x] 3.7 Criar abstração `ICurrentUser` e usar no registro de ocorrência.

## 4. Ajustes de Contrato e Coletas

- [x] 4.1 Remover `ResponsibleUser` de `RegisterIncidentRequest` e DTOs expostos.
- [x] 4.2 Atualizar `CollectionUseCases.RegisterIncidentAsync` para usar usuário autenticado.
- [x] 4.3 Atualizar Swagger com endpoints de auth e contrato revisado de ocorrência.
- [x] 4.4 Regenerar `packages/generated/api-swagger` e `packages/generated/api-types`.

## 5. Frontend de Autenticação

- [x] 5.1 Criar módulo `apps/web/src/modules/auth` (service, hook, página de login).
- [x] 5.2 Implementar chamadas auth com `credentials: 'include'`.
- [x] 5.3 Adicionar rota `/login` e guarda de sessão nas rotas operacionais.
- [x] 5.4 Ajustar `collections.service.ts` para enviar credenciais em todas as requisições.
- [x] 5.5 Remover campo manual de usuário responsável no fluxo de ocorrência.
- [x] 5.6 Exibir usuário autenticado e ação de logout na UI operacional.

## 6. Testes, Documentação e Validação

- [x] 6.2 Criar testes para validators FluentValidation de login e ocorrência em `apps/api/__teste__/Modules/Auth/Application` e `Modules/Collections/Application`.
- [x] 6.3 Criar testes unitários/integrados de auth (login válido/inválido, acesso anônimo) em `Modules/Auth/Application`.
- [x] 6.4 Atualizar testes de ocorrência em `Modules/Collections/Application` para usar usuário autenticado via `ICurrentUser`.
- [x] 6.5 Atualizar `README.md`, `doc/arquitetura.md` e `.env.example` com fluxo de login e variáveis JWT.
- [x] 6.6 Validar manualmente: login → operar coletas → registrar ocorrência → logout → 401.
- [x] 6.7 Executar `dotnet test`, `pnpm nx run web:check-types` e build relevante do monorepo.
