# Arquitetura do Projeto

Esta documentacao resume a arquitetura atual do projeto Transportadora Moura e deve ser usada pelo Cursor como fonte principal de contexto antes de propor mudancas estruturais, alterar contratos entre frontend e backend ou expandir regras de negocio.

Para detalhes de negocio mais extensos, consulte tambem `README.md`, `📐 Regras de Desenvolvimento – Transport.md` e as propostas em `openspec/changes`.

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

A API segue uma Clean Architecture leve, organizada por contexto de dominio:

```text
apps/api/src/
  Domain/
    Coletas/
  Application/
    Coletas/
  Infrastructure/
    Coletas/
  Presentation/
    Coletas/
```

### Domain

`Domain` e o nucleo do sistema. Ele deve concentrar entidades, enums, excecoes de negocio e invariantes.

No contexto de coletas, `Coleta` e o agregado principal. As transicoes de status e validacoes importantes devem permanecer nessa entidade ou em objetos de dominio proximos.

Regras importantes:

- Nao usar EF Core, HTTP, Swagger ou DTOs de API no dominio.
- Evitar setters publicos que permitam burlar invariantes.
- Expressar comportamento com metodos de negocio, como `AtribuirMotoristaVeiculo`, `MarcarEmColeta`, `MarcarComoColetada`, `Cancelar` e `RegistrarOcorrencia`.

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

- `TransportadoraDbContext`.
- Migrations EF Core.
- Repositorios concretos, como `EfColetaRepository`.
- Seed e configuracoes de persistencia.
- Implementacoes tecnicas, como relogio do sistema e gerador de numero.

Infraestrutura implementa contratos definidos na aplicacao. Ela nao deve introduzir regra de negocio escondida.

### Presentation

`Presentation` expoe HTTP por Minimal APIs.

Responsabilidades:

- Endpoints em `ColetasEndpoints`.
- Requests publicos em `ColetaRequests`.
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
Aberta -> EmColeta -> Coletada
Aberta -> Cancelada
EmColeta -> Cancelada
```

Regras centrais:

- Toda coleta nova inicia como `Aberta`.
- `Cancelada` e terminal e deve permanecer registrada para historico.
- `Coletada` tambem encerra o fluxo ativo.
- Nao avancar para `EmColeta` sem motorista e veiculo.
- Nao concluir como `Coletada` sem motorista e veiculo.
- Ocorrencias devem preservar descricao, usuario responsavel e data/hora.
- O backend e a fonte de verdade para validar transicoes; o frontend apenas aciona casos de uso.

## Frontend React

O frontend deve ser organizado por funcionalidades de negocio.

```text
apps/web/src/modules/coletas/
  components/
  hooks/
  pages/
  services/
  types/
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
