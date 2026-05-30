## Why

A API usa apenas o logging padrão do ASP.NET Core (`ILogger` + `appsettings.json`), sem estruturação, enriquecimento de contexto nem sinks configuráveis por ambiente. Isso dificulta diagnosticar falhas em produção, correlacionar requisições HTTP com operações de negócio e evoluir observabilidade (arquivos, níveis por módulo, propriedades estruturadas). A refatoração modular já prevê um bounded context de logging; agora é o momento de implementá-lo com Serilog como padrão único do host.

## What Changes

- Adicionar pacotes Serilog ao `Api.csproj` (host, ASP.NET Core, enriquecimento, sinks Console e File).
- Criar módulo `Modules/Logging` com composição (`AddLoggingModule`, `UseLoggingModule`) e configuração centralizada em inglês.
- Substituir o bootstrap de logging no `Program.cs` por Serilog como provider principal, com flush seguro no shutdown.
- Configurar níveis, formato e sinks via `appsettings.json`, `appsettings.Development.json` e variáveis de ambiente (ex.: `LOG_LEVEL`, `LOG_FILE_PATH`).
- Registrar middleware de request logging (tempo, status, método, path) sem expor dados sensíveis (senhas, tokens).
- Padronizar uso de `ILogger<T>` nos use cases e endpoints existentes, com propriedades estruturadas em eventos de negócio relevantes (criação/atribuição/cancelamento de collection, falhas de autenticação).
- Documentar convenções de log em `doc/arquitetura.md` e variáveis em `.env.example` da API.

## Capabilities

### New Capabilities

- `api-serilog-logging`: logging estruturado na API com Serilog, configuração por ambiente, request logging e convenções para módulos de negócio.

### Modified Capabilities

Nenhuma. Contratos HTTP, regras de negócio e respostas da API permanecem inalterados; apenas observabilidade interna melhora.

## Impact

- Backend .NET: novo `Modules/Logging`, alterações em `Program.cs`, `Composition/`, `appsettings*.json`, `Api.csproj`, possíveis logs pontuais em `Modules/Collections` e `Modules/Auth`.
- Dependências NuGet: Serilog e extensões oficiais.
- Operações locais: pasta/arquivo de log opcional em desenvolvimento; documentação de variáveis de ambiente.
- Sem impacto no frontend, OpenAPI ou pacotes gerados.
- Testes: validar que a aplicação inicia com Serilog configurado; testes unitários não devem depender de sinks em disco (usar `NullLogger` ou config mínima em testes).
