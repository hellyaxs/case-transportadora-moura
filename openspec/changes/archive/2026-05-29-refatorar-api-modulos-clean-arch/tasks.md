## 1. Estrutura Base e Shared

- [x] 1.1 Criar pastas `Shared`, `Modules`, `Composition` em `apps/api/src`.
- [x] 1.2 Mover `BusinessRuleException` e demais cross-cutting de domínio para `Shared/Domain`.
- [x] 1.3 Mover `DotEnvLoader`, `PostgresConnectionStringFactory` e `TransportadoraDbContext` para `Shared/Infrastructure`.
- [x] 1.4 Ajustar namespaces para `Api.Shared.*` e garantir build parcial.

## 2. Módulo Coletas

- [x] 2.1 Mover `Domain/Coletas` para `Modules/Coletas/Domain` com namespaces `Api.Modules.Coletas.Domain`.
- [x] 2.2 Mover `Application/Coletas` para `Modules/Coletas/Application`.
- [x] 2.3 Mover `Infrastructure/Coletas` para `Modules/Coletas/Infrastructure`.
- [x] 2.4 Mover `Presentation/Coletas` para `Modules/Coletas/Presentation`.
- [x] 2.5 Extrair configurações EF de coletas para classes de configuração do módulo aplicadas no DbContext compartilhado.
- [x] 2.6 Criar `ColetasModule.cs` com `AddColetasModule` e `MapColetasEndpoints`.

## 3. Esqueleto do Módulo Auth

- [x] 3.1 Criar estrutura `Modules/Auth/{Domain,Application,Infrastructure,Presentation}`.
- [x] 3.2 Criar `AuthModule.cs` com `AddAuthModule` e `MapAuthEndpoints` stub (sem lógica de negócio).
- [x] 3.3 Documentar no módulo que a implementação JWT ocorrerá em `autenticacao-jwt-httponly`.

## 4. Composition Root

- [x] 4.1 Criar `Composition/DependencyInjection.cs` agregando `AddColetasModule` e `AddAuthModule`.
- [x] 4.2 Criar `Composition/EndpointRegistration.cs` agregando mapeamentos de endpoints.
- [x] 4.3 Refatorar `Program.cs` para usar apenas composição, CORS, Swagger e migrate.

## 5. Testes por Módulo

- [x] 5.1 Criar estrutura `apps/api/__teste__/Modules/Coletas/{Domain,Application}` e `Modules/Auth/Application`.
- [x] 5.2 Mover testes de domínio existentes para `Modules/Coletas/Domain` (ex.: `ColetaDomainTests.cs`).
- [x] 5.3 Criar `ColetaUseCasesTests.cs` em `Modules/Coletas/Application` com mocks de `IColetaRepository`, `IClock` e `IColetaNumeroGenerator`.
- [x] 5.4 Cobrir casos de uso principais: criar coleta, atribuir, registrar ocorrência, cancelar e bloqueios de regra de negócio delegados ao domínio.
- [x] 5.5 Criar esqueleto/documentação de testes em `Modules/Auth/Application` para a change de autenticação.
- [x] 5.6 Extrair fakes/helpers compartilhados para `__teste__/Shared` apenas se necessário (evitar acoplamento entre módulos).

## 6. Docs e Validação

- [x] 6.1 Atualizar `doc/arquitetura.md` e regras Cursor de backend com estrutura modular e convenção de testes por módulo.
- [x] 6.2 Executar `dotnet build apps/api/Api.csproj` e `dotnet test apps/api/__teste__/Api.Test.csproj`.
- [x] 6.3 Validar Swagger e smoke manual dos endpoints de coletas/cadastros após refactor.
- [x] 6.4 Confirmar que nenhum contrato HTTP público foi alterado.

## 7. Sequenciamento com Autenticação

- [x] 7.1 Registrar na change `autenticacao-jwt-httponly` que testes de auth devem viver em `__teste__/Modules/Auth/Application`.
