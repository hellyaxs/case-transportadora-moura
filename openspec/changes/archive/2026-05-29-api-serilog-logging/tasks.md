## 1. Dependências e estrutura do módulo

- [x] 1.1 Adicionar pacotes Serilog em `apps/api/Api.csproj` (`Serilog.AspNetCore`, `Serilog.Settings.Configuration`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`, enrichers opcionais).
- [x] 1.2 Criar `apps/api/src/Modules/Logging/` com `LoggingModule.cs` (`AddLoggingModule`, `UseLoggingModule`) e classes de configuração em inglês.
- [x] 1.3 Registrar o módulo em `Composition/DependencyInjection.cs` e no pipeline HTTP (extensão de composição).

## 2. Bootstrap Serilog e configuração

- [x] 2.1 Configurar bootstrap Serilog no `Program.cs` (`Log.Logger`, `UseSerilog`, `CloseAndFlush` no shutdown).
- [x] 2.2 Adicionar seção `Serilog` em `appsettings.json` e overrides em `appsettings.Development.json`.
- [x] 2.3 Mapear variáveis `LOG_LEVEL` e `LOG_FILE_PATH` via `DotEnvLoader` / configuration binding.
- [x] 2.4 Documentar variáveis em `apps/api/.env.example`.

## 3. Request logging e pipeline

- [x] 3.1 Aplicar `UseSerilogRequestLogging` com enriquecimento de `TraceIdentifier`.
- [x] 3.2 Filtrar ou reduzir ruído de `/swagger` e assets estáticos em desenvolvimento.
- [x] 3.3 Garantir ordem correta no pipeline (após CORS, antes de `MapApiModules`).

## 4. Logs de negócio e segurança

- [x] 4.1 Injetar `ILogger<T>` nos use cases de Auth e adicionar log de falha de login (sem senha/token).
- [x] 4.2 Adicionar logs de erro inesperado ou eventos relevantes em Collections (cancelamento, falha de regra crítica) sem destructuring de DTOs sensíveis.
- [x] 4.3 Revisar middleware/handlers globais para logar exceções não tratadas antes da resposta HTTP.

## 5. Documentação e validação

- [x] 5.1 Atualizar `doc/arquitetura.md` com seção do módulo Logging, pacotes e convenções.
- [x] 5.2 Adicionar `logs/` ou padrão de arquivo ao `.gitignore` se file sink for usado localmente.
- [x] 5.3 Validar startup local (`pnpm dev:api`) com Console e, opcionalmente, arquivo de log.
- [x] 5.4 Executar `dotnet build` e `dotnet test` em `apps/api` sem regressões.
