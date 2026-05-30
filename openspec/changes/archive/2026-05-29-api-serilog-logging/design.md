## Context

A API ASP.NET Core 8 roda como modular monolith (`Modules/Collections`, `Modules/Auth`, `Shared`, `Composition`). O host usa `WebApplication.CreateBuilder` com logging padrão Microsoft (`Logging:LogLevel` em `appsettings.json`). Não há Serilog, request logging estruturado nem convenção documentada para eventos de negócio.

A refatoração `refatorar-api-modulos-clean-arch` já reservou a convenção `AddXModule` / `MapXModule` para futuros módulos, incluindo logging. Esta change implementa esse módulo sem alterar contratos REST.

## Goals / Non-Goals

**Goals:**

- Serilog como único pipeline de log do host (bootstrap + `UseSerilogRequestLogging` ou equivalente).
- Módulo `Modules/Logging` com extensões de composição reutilizáveis.
- Logs estruturados (propriedades nomeadas) em Console e, opcionalmente, arquivo rotativo em desenvolvimento.
- Request logging com duração, status HTTP, método e path; exclusão de headers/corpo sensíveis.
- Configuração por `appsettings`, ambiente e variáveis de ambiente alinhadas ao padrão do projeto (`DotEnvLoader`).
- `ILogger<T>` continua sendo a abstração injetada em use cases; Serilog fica por baixo via `Microsoft.Extensions.Logging`.
- Logs pontuais em fluxos críticos (auth failure, collection lifecycle errors) sem poluir happy path.

**Non-Goals:**

- Não integrar APM externo (Application Insights, Datadog, Seq cloud) nesta fase.
- Não persistir logs em banco PostgreSQL.
- Não expor endpoint HTTP de consulta de logs.
- Não alterar respostas de erro da API para o cliente.
- Não exigir Serilog em testes unitários (permanecem com `NullLogger` ou logging desabilitado).

## Decisions

### Módulo `Modules/Logging` vs apenas `Shared`

**Decisão:** criar `Modules/Logging` com `LoggingModule.cs` (`AddLoggingModule`, `UseLoggingModule`), registrado em `Composition/DependencyInjection.cs` e `EndpointRegistration.cs` (ou extensão dedicada no pipeline).

**Rationale:** alinha com bounded context de observabilidade previsto na arquitetura modular; `Shared` permanece para DbContext e config, não para bootstrap de infra do host.

**Alternativa rejeitada:** colocar tudo em `Shared/Infrastructure` — mistura cross-cutting de persistência com observabilidade.

### Pacotes Serilog

**Decisão:** usar pacotes oficiais estáveis para .NET 8:

- `Serilog.AspNetCore` (integração host + request logging)
- `Serilog.Settings.Configuration` (bind de `Serilog` section)
- `Serilog.Enrichers.Environment`, `Serilog.Enrichers.Thread` (opcional, baixo custo)
- Sinks: `Serilog.Sinks.Console`, `Serilog.Sinks.File` (rotação diária em dev)

**Alternativa rejeitada:** sink PostgreSQL — complexidade operacional desnecessária no desafio.

### Bootstrap no `Program.cs`

**Decisão:** configurar Serilog **antes** de `WebApplication.CreateBuilder` (padrão recomendado) com `Log.Logger` inicial e `builder.Host.UseSerilog()`, garantindo `Log.CloseAndFlush()` em `finally` no shutdown.

**Rationale:** captura falhas de startup (migrations, DI) que ocorrem antes do pipeline HTTP.

### Configuração

**Decisão:** seção `Serilog` em `appsettings.json` e override em `appsettings.Development.json`. Variáveis de ambiente mapeadas:

| Env | Efeito |
|-----|--------|
| `LOG_LEVEL` | Minimum level global (ex.: `Debug`, `Information`) |
| `LOG_FILE_PATH` | Caminho do arquivo; vazio = sem file sink |
| `ASPNETCORE_ENVIRONMENT` | já existente; controla template verboso |

Manter seção `Logging` do Microsoft apenas se necessário para bibliotecas de terceiros; Serilog é a fonte da verdade.

**Formato:** Console em dev com template legível; JSON compacto opcional via config (não obrigatório na primeira entrega).

### Request logging

**Decisão:** `app.UseSerilogRequestLogging()` no pipeline, após `UseCors` e antes de endpoints, com filtro que não loga health/swagger em nível Information repetitivo (opcional: downgrade `/swagger`).

Enriquecer com `RequestId` / `TraceIdentifier` do ASP.NET Core para correlação.

### Dados sensíveis

**Decisão:** nunca logar corpo de login, `Authorization`, cookies JWT ou connection strings. Destructuring de objetos de request DTOs proibido em nível Debug sem `[LogMasked]` ou log manual de IDs apenas.

### Uso em módulos de negócio

**Decisão:** injetar `ILogger<T>` nos use cases já existentes onde fizer sentido (erros de `BusinessRuleException` podem permanecer sem log se tratados no middleware global; logar exceções inesperadas e eventos de auditoria leve: login falho, collection cancelada).

**Alternativa rejeitada:** wrapper custom `IAppLogger` — adiciona camada sem ganho imediato; Serilog já integra com `ILogger`.

### Testes

**Decisão:** testes em `apps/api/__teste__` não alteram host real; se houver teste de integração futuro, usar `Serilog.Sinks.InMemory` ou logging mínimo. Nenhum teste deve escrever em disco compartilhado.

## Risks / Trade-offs

| Risco | Mitigação |
|-------|-----------|
| Volume de logs em dev (request por chamada) | Filtrar swagger/static; nível `Warning` para Microsoft.AspNetCore em prod |
| Arquivo de log crescer sem rotação | `rollingInterval: Day` e tamanho máximo configurável |
| Duplicidade Microsoft + Serilog | `UseSerilog()` substitui providers padrão; limpar config conflitante |
| PII em logs | Checklist na spec; revisão em PR de endpoints Auth |
| Falha de escrita em arquivo | Sink file opcional; app continua com Console |

## Migration Plan

1. Adicionar pacotes e módulo Logging sem remover `ILogger` existente.
2. Configurar Serilog no host; validar startup local (`pnpm dev:api`).
3. Adicionar logs pontuais em Auth/Collections.
4. Atualizar `doc/arquitetura.md` e `.env.example`.
5. Rollback: reverter `UseSerilog` e pacotes; `appsettings` volta ao logging Microsoft nativo.

## Open Questions

- Em produção futura, adotar sink JSON para agregador (Loki/ELK)? Deixar preparado via config, sem implementar agora.
- Nível padrão em CI: `Warning` para reduzir ruído nos testes de build?
