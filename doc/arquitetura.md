# Arquitetura do Projeto

Esta documentacao resume a arquitetura atual do projeto Transportadora Moura e deve ser usada pelo Cursor como fonte principal de contexto antes de propor mudancas estruturais, alterar contratos entre frontend e backend ou expandir regras de negocio.

Para detalhes de negocio mais extensos, consulte tambem as propostas em `openspec/changes`.

## Visao Geral

O projeto e um monorepo orquestrado por Nx e pnpm. Ele combina uma API .NET para gestao operacional de coletas, uma aplicacao web React, pacotes compartilhados e artefatos gerados a partir do contrato OpenAPI.

```text
transportadora-moura/
  apps/
    api/                 # API ASP.NET Core Minimal API
    web/                 # Frontend React + Vite + TanStack Router
  packages/
    config/              # Configuracoes compartilhadas TypeScript
    env/                 # Validacao de variaveis de ambiente do frontend
    ui/                  # Componentes shadcn/ui compartilhados
    generated/
      api-swagger/       # Swagger/OpenAPI gerado pela API
      api-types/         # Cliente/tipos TypeScript gerados do OpenAPI
  openspec/              # Propostas, designs, specs e tarefas do produto
```

## Responsabilidades por Area

`apps/api` concentra o backend .NET. A API usa ASP.NET Core Minimal API, Entity Framework Core com Npgsql, Swagger e migracoes para PostgreSQL. O codigo fica organizado em camadas de dominio, aplicacao, infraestrutura e apresentacao.

`apps/web` concentra o frontend. A aplicacao usa React, Vite, TanStack Router e Tailwind. Funcionalidades de negocio devem ficar em `apps/web/src/modules/<dominio>`.

`packages/ui` fornece componentes compartilhados baseados em shadcn/ui. Componentes que sao realmente reutilizaveis entre apps devem viver aqui; componentes especificos de uma tela devem permanecer no app.

`packages/env` valida variaveis de ambiente do frontend com `@t3-oss/env-core`. Atualmente o frontend depende de `VITE_API_BASE_URL`.

`packages/generated/api-swagger` armazena o arquivo Swagger/OpenAPI gerado a partir da API.

`packages/generated/api-types` contem tipos e APIs TypeScript gerados a partir do Swagger. Nao edite estes arquivos manualmente quando a mudanca deve vir do contrato da API.

`openspec` guarda especificacoes de produto e infraestrutura. Use esse diretorio para entender intencao, escopo e requisitos antes de alterar fluxos relevantes.

## Fluxo Local

Comandos principais:

```bash
pnpm install
docker compose --env-file apps/api/.env up -d postgres
pnpm dev:api
pnpm dev:web
```

Servicos locais esperados:

- PostgreSQL: `localhost:5432`
- API: `http://localhost:5086`
- Swagger: `http://localhost:5086/swagger`
- Web: verificar a porta configurada em `apps/web/vite.config.ts`

Observacao: o `README.md` cita a web em `5173`, mas `apps/web/vite.config.ts` configura `3001`. Antes de documentar URLs finais ou configurar CORS, confirme qual porta deve ser considerada fonte da verdade.

Outros comandos uteis:

```bash
pnpm build
pnpm check-types
pnpm nx run api:swagger
pnpm nx run api-swagger:codegen
dotnet test apps/api.Test/Api.Test.csproj
```

## Backend .NET

A API segue uma Clean Architecture leve, organizada por **modulos de bounded context**, cada um com suas proprias camadas:

```text
apps/api/src/
  Shared/
    Domain/Exceptions/
    Infrastructure/
      Configuration/
      Persistence/TransportadoraDbContext.cs
  Modules/
    Collections/
      Domain/
      Application/
      Infrastructure/
      Presentation/
      CollectionsModule.cs
    Auth/
      Domain/
      Application/
      Infrastructure/
      Presentation/
      AuthModule.cs
  Composition/
    DependencyInjection.cs
    EndpointRegistration.cs

apps/api/__teste__/
  Modules/
    Collections/
      Domain/
      Application/
    Auth/
      Application/
```

Cada modulo expoe `AddXModule` e `MapXModule`. O `Program.cs` atua apenas como composition root via `AddApiModules()` e `MapApiModules()`.

Testes unitarios ficam organizados por modulo em `apps/api/__teste__/Modules/<Modulo>/`, cobrindo dominio e casos de uso da Application com dependencias mockadas.

### Domain

`Domain` e o nucleo do sistema. Ele deve concentrar entidades, enums, excecoes de negocio e invariantes.

No contexto de coletas, `Collection` e o agregado principal. As transicoes de status e validacoes importantes devem permanecer nessa entidade ou em objetos de dominio proximos.

Regras importantes:

- Nao usar EF Core, HTTP, Swagger ou DTOs de API no dominio.
- Evitar setters publicos que permitam burlar invariantes.
- Expressar comportamento com metodos de negocio em ingles, como `AssignDriverAndVehicle`, `MarkInProgress`, `MarkAsCollected`, `Cancel` e `RegisterIncident`.

### Application

`Application` orquestra casos de uso e depende de abstracoes. Ela chama o dominio, mas nao substitui as regras do dominio.

Responsabilidades:

- Casos de uso em `UseCases`.
- DTOs de aplicacao em `Dtos`.
- Contratos como repositorios, relogio e geradores em `Contracts`.
- Coordenacao de fluxo entre dominio e persistencia.

### Infrastructure

`Infrastructure` contem detalhes tecnicos.

Responsabilidades:

- `TransportadoraDbContext` em `Shared/Infrastructure/Persistence`.
- Migrations EF Core em `apps/api/Migrations`.
- Configuracoes EF por modulo (ex.: `Modules/Collections/Infrastructure/Persistence/Configurations`).
- Repositorios concretos, como `EfCollectionRepository`.
- Seed e configuracoes de persistencia por modulo.
- Implementacoes tecnicas, como relogio do sistema e gerador de numero.

Infraestrutura implementa contratos definidos na aplicacao. Ela nao deve introduzir regra de negocio escondida.

### Logging (Módulo de Observabilidade)

O módulo `Modules/Logging` centraliza a configuração do Serilog como pipeline de logging do host.

Estrutura:

```text
apps/api/src/Modules/Logging/
  LoggingModule.cs        # AddLoggingModule / UseLoggingModule
  LoggingOptions.cs       # Configuração tipada da seção Serilog
```

Pacotes NuGet adicionados:

- `Serilog.AspNetCore` — integração com host ASP.NET Core e request logging.
- `Serilog.Settings.Configuration` — bind da seção `Serilog` do `appsettings.json`.
- `Serilog.Sinks.Console` — saída padrão em console.
- `Serilog.Sinks.File` — arquivo rotativo opcional (desenvolvimento).
- `Serilog.Enrichers.Environment` — enriquecimento com `MachineName`.
- `Serilog.Enrichers.Thread` — enriquecimento com `ThreadId`.

Convenções:

- **Bootstrap**: `Log.Logger` é configurado antes de `WebApplication.CreateBuilder` com bootstrap logger; o host usa `UseSerilog()` para aplicar a configuração completa lida do `appsettings.json`.
- **Request logging**: adicionado via `UseSerilogRequestLogging` com enriquecimento do `TraceIdentifier`. Requisições para `/swagger` e `/favicon.ico` são rebaixadas para `Verbose` para reduzir ruído.
- **Configuração**: seção `Serilog` nos `appsettings*.json` com suporte a override por variáveis de ambiente `LOG_LEVEL` e `LOG_FILE_PATH`.
- **Dados sensíveis**: proibido logar corpo de login, tokens JWT, `Authorization` header ou connection strings. Logs de negócio usam apenas IDs, nomes e status.
- **ILogger\<T\>**: use cases continuam injetando `ILogger<T>` da Microsoft; Serilog recebe os eventos via bridge do `Microsoft.Extensions.Logging`.
- **Middleware de exceção**: `ExceptionHandlingMiddleware` global captura exceções não tratadas, registra no Serilog e retorna resposta `problem+json` adequada.

### Presentation

`Presentation` expoe HTTP por Minimal APIs.

Responsabilidades:

- Endpoints em `CollectionsEndpoints`.
- Requests publicos em `CollectionRequests`.
- Mapeamento entre entrada HTTP e casos de uso.
- Documentacao Swagger/OpenAPI.
- Conversao de erros de negocio para respostas HTTP claras.

Presentation nao deve expor entidades de dominio diretamente nem conter transicoes de status ou regras operacionais.

## Dominio de Coletas

Use a linguagem do dominio de forma consistente:

- `Coleta`: solicitacao operacional para retirada de carga.
- `Cliente`: solicitante da coleta.
- `Remetente`: origem da carga.
- `Destinatario`: destino ou recebedor.
- `Motorista`: responsavel pela execucao.
- `Veiculo`: recurso atribuido para execucao.
- `Ocorrencia`: evento registrado durante o ciclo da coleta.
- `Prioridade`: classificacao operacional.
- `Status`: etapa atual do fluxo.

Fluxo base:

```text
Open -> InProgress -> Collected
Open -> Cancelled
InProgress -> Cancelled
```

Regras centrais:

- Toda coleta nova inicia como `Open`.
- `Cancelled` e terminal e deve permanecer registrada para historico.
- `Collected` tambem encerra o fluxo ativo.
- Nao avancar para `InProgress` sem motorista e veiculo.
- Nao concluir como `Collected` sem motorista e veiculo.
- Ocorrencias devem preservar descricao, usuario responsavel e data/hora.
- O backend e a fonte de verdade para validar transicoes; o frontend apenas aciona casos de uso.

## Nomenclatura em ingles (codigo e contratos)

Todo codigo executavel, banco e contratos HTTP usam ingles. Specs OpenSpec e esta documentacao podem permanecer em portugues.

Glossario produto (PT) → tecnico (EN):

| Produto (PT) | Codigo / API / DB (EN) |
|--------------|-------------------------|
| Coleta | `Collection` |
| Cliente | `Customer` |
| Motorista | `Driver` |
| Veiculo | `Vehicle` |
| Ocorrencia | `CollectionIncident` |
| Aberta / Em coleta / Coletada / Cancelada | `Open` / `InProgress` / `Collected` / `Cancelled` |
| Normal / Alta | `Normal` / `High` |

Rotas REST publicas:

| Recurso | Rota |
|---------|------|
| Coletas | `/api/collections` |
| Clientes | `/api/customers` |
| Motoristas | `/api/drivers` |
| Veiculos | `/api/vehicles` |
| Autenticacao | `/api/auth/login`, `/api/auth/logout`, `/api/auth/me` |

Acoes de fluxo em coletas: `POST .../assignment`, `.../start`, `.../complete`, `.../cancel`, `.../incidents`.

### Autenticacao JWT (cookie HttpOnly)

- Modulo `Modules/Auth` com entidade `User`, seed demo (`operador@moura.local` / `Moura@2026`), JWT assinado e cookie `auth_token` (`HttpOnly`, `SameSite=Lax` em dev HTTP).
- Endpoints operacionais de coletas e cadastros exigem `RequireAuthorization()`.
- Ocorrencias usam `ICurrentUser` (Shared/Application); `RegisterIncidentRequest` aceita apenas `description`.
- Frontend: `apps/web/src/modules/auth`, rota `/login`, `credentials: 'include'` nos services.
- Variaveis: `JWT_SECRET` (obrigatoria), `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpirationHours`, `Jwt__CookieName` em `apps/api/.env`.
- CORS com `AllowCredentials()` e `FRONTEND_ORIGIN` explicito (ex.: `http://localhost:3001`).

Consulte `.cursor/rules/english-codebase.mdc` para regras permanentes do Cursor.

## Frontend React

O frontend deve ser organizado por funcionalidades de negocio.

```text
apps/web/src/modules/collections/
  components/
  hooks/
  pages/
  services/
  types/

apps/web/src/modules/auth/
  hooks/auth-provider.tsx
  pages/login-page.tsx
  services/auth.service.ts
```

Padroes:

- Pages montam a experiencia da rota.
- Components renderizam UI e recebem dados/acoes por props.
- Hooks encapsulam estado, filtros, carregamento, erros e acoes da tela.
- Services fazem chamadas HTTP para a API.
- Types locais representam estado de UI quando os tipos gerados nao forem suficientes.

Componentes visuais nao devem decidir regra de negocio. Eles podem esconder acoes invalidas para melhorar UX, mas o backend continua validando o fluxo.

## Integracao Frontend e Backend

O backend define o contrato publico via Swagger/OpenAPI. O frontend deve consumir os tipos gerados em `packages/generated/api-types` quando existirem.

Fluxo esperado quando mudar DTOs, endpoints ou enums publicos:

```text
apps/api -> Swagger/OpenAPI -> packages/generated/api-types -> apps/web
```

Comandos relacionados:

```bash
pnpm nx run api:swagger
pnpm nx run api-swagger:codegen
```

Regras:

- Nao duplicar manualmente enums ou DTOs que ja existem nos tipos gerados.
- Mudancas em requests/responses publicos devem atualizar Swagger e tipos gerados.
- O frontend deve tratar mensagens de erro de negocio de forma compreensivel para o operador.
- Arquivos gerados devem ser tratados como artefatos de contrato, nao como fonte manual de regra.

## Migrations e Nomenclatura

Migrations EF Core ficam em `apps/api/Migrations` e seguem o padrao gerado pelo EF: `yyyyMMddHHmmss_NomeDaMigration.cs`, `yyyyMMddHHmmss_NomeDaMigration.Designer.cs` e `TransportadoraDbContextModelSnapshot.cs`.

Antes de criar uma migration, altere entidades e configuracoes em `TransportadoraDbContext`. Depois de gerar, revise `Up`, `Down`, seed, indices e possiveis operacoes destrutivas.

Use nomes de migration em `PascalCase`, descritivos e orientados a mudanca, como `InitialColetas`, `AddColetaIndexes` ou `AddOcorrenciaUsuarioResponsavel`.

Nomenclatura geral:

- Backend C#: arquivos manuais em `PascalCase`, interfaces com prefixo `I`, classes/enums/exceptions em `PascalCase`.
- Frontend React: arquivos manuais em `kebab-case`, componentes em `PascalCase`, hooks com prefixo `use` e services como `<dominio>.service.ts`.
- Arquivos gerados ou convencionais podem fugir da regra, como `routeTree.gen.ts`, `index.tsx`, migrations `.Designer.cs` e snapshots do EF Core.

Consulte tambem `.cursor/rules/backend-migrations.mdc` e `.cursor/rules/file-naming.mdc`.

## OpenSpec

`openspec/changes` documenta intencao, design, tarefas e requisitos. Antes de mudar comportamento de negocio, consulte os specs relacionados.

Mudancas relevantes atuais:

- `openspec/changes/gestao-coletas-transportadora`
- `openspec/changes/infra-postgres-integracao-front-back`

Observacao: algumas tarefas podem estar marcadas como pendentes mesmo quando o codigo ja implementa parte do comportamento. Sempre confronte OpenSpec com o estado atual do codigo antes de assumir que algo falta.

## Testes e Validacao

Para mudancas de dominio ou aplicacao backend, priorize testes unitarios em `apps/api.Test`, especialmente para invariantes de `Coleta` e casos de uso.

Para mudancas de contrato, valide Swagger/codegen e cheque se o frontend continua usando tipos gerados.

Para mudancas de frontend, rode ao menos checagem de tipos quando houver alteracao TypeScript relevante:

```bash
pnpm check-types
```

## Regras para o Cursor

Quando trabalhar neste projeto:

- Consulte esta documentacao antes de alterar arquitetura, contratos, fluxos de coletas ou organizacao de modulos.
- Prefira seguir padroes locais a criar novas abstracoes.
- Mantenha regras de negocio no backend, especialmente no dominio.
- Use OpenSpec para entender intencao de produto.
- Atualize esta documentacao quando fizer mudanca estrutural que afete futuras decisoes.
