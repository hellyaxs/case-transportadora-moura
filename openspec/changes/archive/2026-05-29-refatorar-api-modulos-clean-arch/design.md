## Context

A API ASP.NET Core atual usa Clean Architecture leve, mas organizada por camada técnica com um único subcontexto (`Coletas`). O frontend React já adota `modules/<dominio>`. Próximas entregas incluem autenticação JWT (`autenticacao-jwt-httponly`) e, futuramente, logging como módulo separado.

A refatoração deve preservar comportamento, contratos REST, migrations existentes e testes unitários de coletas, alterando apenas estrutura e composição.

## Goals / Non-Goals

**Goals:**

- Adotar **modular monolith** com bounded contexts isolados por pasta e namespace.
- Manter Clean Architecture **dentro de cada módulo** (`Domain`, `Application`, `Infrastructure`, `Presentation`).
- Mover coletas/transporte para `Modules/Coletas` com paridade funcional.
- Centralizar cross-cutting em `Shared` (config, exceções base, composição de DbContext).
- Criar esqueleto de `Modules/Auth` pronto para receber usuários, JWT e endpoints na change seguinte.
- Definir convenção replicável para futuros módulos (`Logging`, etc.).
- Organizar testes unitários **por módulo**, cobrindo casos de uso da Application com mocks/fakes das dependências externas.

**Non-Goals:**

- Não implementar autenticação, logging ou novos endpoints nesta change.
- Não alterar contratos HTTP, DTOs públicos ou regras de negócio de coletas.
- Não dividir em múltiplos projetos .csproj nesta fase (permanece um `Api.csproj`).
- Não introduzir message bus, microservices ou bancos separados por módulo.

## Decisions

### Estrutura alvo de pastas

```text
apps/api/src/
  Shared/
    Domain/
      Exceptions/
    Infrastructure/
      Configuration/
      Persistence/
        TransportadoraDbContext.cs
  Modules/
    Coletas/
      Domain/
      Application/
      Infrastructure/
      Presentation/
      ColetasModule.cs
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
    Coletas/
      Domain/
      Application/
    Auth/
      Application/
  Shared/
```

Alternativa considerada: múltiplos projetos (`Api.Coletas`, `Api.Auth`). Rejeitada por aumentar complexidade de build/Nx sem necessidade imediata.

### Testes unitários por módulo

Cada módulo SHALL possuir testes no projeto `Api.Test`, organizados em `__teste__/Modules/<Modulo>/`, espelhando as camadas testadas:

- **Domain:** invariantes e transições (já existentes para coletas).
- **Application:** casos de uso (`ColetaUseCases`, futuros use cases de Auth) com repositórios/contratos mockados — sem EF, HTTP ou DbContext.

Convenção de nomenclatura: `<UseCase>Tests.cs` ou `<Entidade>Tests.cs` dentro da pasta da camada correspondente.

Alternativa considerada: manter todos os testes em `UnitTest1.cs` na raiz de `__teste__`. Rejeitada por não escalar com Auth/Logging e misturar bounded contexts.

Alternativa considerada: projeto de testes por módulo (`Api.Coletas.Test.csproj`). Rejeitada nesta fase; manter um `Api.Test.csproj` com pastas por módulo reduz complexidade de build.

### Namespaces alinhados ao módulo

Adotar namespaces como `Api.Modules.Coletas.Domain`, `Api.Modules.Auth.Application`, `Api.Shared.Infrastructure`. Isso deixa explícito o boundary e evita colisão quando Auth e Coletas crescerem.

Alternativa considerada: manter `Api.Domain.Coletas`. Rejeitada por não escalar quando Auth/Logging forem adicionados no mesmo nível.

### DbContext compartilhado com configurações por módulo

Manter um único `TransportadoraDbContext` em `Shared.Infrastructure.Persistence`, aplicando `IEntityTypeConfiguration<>` de cada módulo (ex.: configurações de Coletas em `Modules/Coletas/Infrastructure/Persistence`).

Alternativa considerada: DbContext por módulo. Rejeitada nesta fase por exigir múltiplas connections/migrations sem benefício imediato.

### Composition root explícita

Cada módulo expõe métodos de extensão:

- `IServiceCollection AddColetasModule(...)`
- `IServiceCollection AddAuthModule(...)` (stub)
- `WebApplication MapColetasEndpoints(...)`
- `WebApplication MapAuthEndpoints(...)` (stub)

`Program.cs` apenas carrega env, chama `AddApiModules()` e `MapApiModules()`.

Alternativa considerada: registrar tudo inline em `Program.cs`. Rejeitada por não escalar com novos módulos.

### Esqueleto de Auth sem comportamento

`Modules/Auth` recebe estrutura completa de camadas e registros vazios/documentados, sem entidades ou endpoints reais. A change `autenticacao-jwt-httponly` preencherá esse módulo.

Alternativa considerada: criar Auth apenas na change seguinte. Rejeitada porque o usuário pediu preparar o módulo antes da autenticação.

### Paridade funcional garantida por testes

Testes de domínio existentes devem continuar passando após a movimentação. Nesta change, adicionar testes unitários de `ColetaUseCases` cobrindo orquestração principal (criar, atribuir, ocorrência, cancelar) com mocks de `IColetaRepository`, `IClock` e `IColetaNumeroGenerator`.

## Risks / Trade-offs

- Refactor grande tocando muitos arquivos → Mitigar com movimentação mecânica, commits focados e execução de `dotnet test` ao final.
- Namespaces quebrados em imports → Mitigar atualizando testes e busca global antes de concluir.
- DbContext monolítico pode crescer demais → Mitigar com configurações por módulo e convenção para futura separação se necessário.
- Esqueleto Auth vazio pode confundir → Mitigar com comentário/README curto no módulo indicando implementação na change seguinte.

## Migration Plan

1. Criar pastas `Shared`, `Modules/Coletas`, `Modules/Auth` e `Composition`.
2. Mover código de coletas para `Modules/Coletas` preservando lógica.
3. Mover cross-cutting para `Shared`.
4. Extrair extension methods de DI/endpoints por módulo.
5. Enxugar `Program.cs`.
6. Criar stub de `AuthModule`.
7. Reorganizar testes por módulo e adicionar testes unitários de casos de uso de Coletas.
8. Atualizar arquitetura e validar build/testes/Swagger.

Rollback: revert do commit de refatoração; não há migration de banco nesta change.

## Open Questions

- Nome final do módulo operacional: manter `Coletas` ou renomear para `Transporte`/`Operacao`? **Sugestão:** manter `Coletas` nesta change para reduzir diff; renomear depois se necessário.
- `Shared` deve virar projeto separado no futuro quando Logging exigir dependências próprias?
