# API Modular Structure

## Purpose

Definir a estrutura modular da API usando bounded contexts e Clean Architecture para separar regras de negócio, casos de uso, infraestrutura, apresentação HTTP e componentes compartilhados.

## Requirements

### Requirement: API organizada por módulos de bounded context

A API SHALL organizar código de negócio em módulos verticais independentes, em vez de agrupar apenas por camada técnica na raiz de `src`.

#### Scenario: Estrutura modular presente

- **WHEN** um desenvolvedor inspeciona `apps/api/src`
- **THEN** encontra módulos dedicados como `Modules/Coletas` e `Modules/Auth` com camadas internas próprias

### Requirement: Clean Architecture dentro de cada módulo

Cada módulo de negócio SHALL conter pastas `Domain`, `Application`, `Infrastructure` e `Presentation` para isolar regras, casos de uso, detalhes técnicos e HTTP.

#### Scenario: Módulo Coletas reorganizado

- **WHEN** o módulo de coletas é inspecionado após a refatoração
- **THEN** suas entidades, use cases, repositórios e endpoints residem dentro de `Modules/Coletas` respeitando a separação de camadas

### Requirement: Infraestrutura compartilhada isolada em Shared

A API SHALL concentrar cross-cutting reutilizável (configuração base, DbContext compartilhado, exceções comuns) em `Shared`, sem misturar regra de negócio específica de um módulo.

#### Scenario: DbContext compartilhado

- **WHEN** módulos precisam persistir dados no mesmo banco
- **THEN** eles utilizam infraestrutura compartilhada em `Shared` sem duplicar configuração de conexão

### Requirement: Composition root centralizada

O host da API SHALL compor módulos por extension methods de DI e mapeamento de endpoints, mantendo `Program.cs` como ponto de composição.

#### Scenario: Registro de módulos

- **WHEN** a aplicação inicia
- **THEN** serviços e endpoints de cada módulo são registrados via métodos de composição dedicados

### Requirement: Esqueleto do módulo Auth preparado

A API SHALL disponibilizar estrutura inicial do módulo `Auth` com camadas e registro de composição, mesmo antes da implementação de autenticação JWT.

#### Scenario: Módulo Auth inspecionado

- **WHEN** um desenvolvedor abre `Modules/Auth`
- **THEN** encontra estrutura de Clean Architecture e ponto de registro pronto para receber a change de autenticação

### Requirement: Paridade funcional dos endpoints de coletas

A refatoração estrutural SHALL preservar o comportamento público dos endpoints existentes de coletas e cadastros operacionais.

#### Scenario: Contratos HTTP inalterados

- **WHEN** a refatoração é concluída
- **THEN** paths, métodos HTTP e payloads públicos de coletas permanecem equivalentes aos existentes antes da mudança

#### Scenario: Testes de coletas continuam válidos

- **WHEN** a suíte de testes de coletas é executada após a refatoração
- **THEN** os cenários existentes continuam passando sem alteração de regra de negócio

### Requirement: Testes unitários organizados por módulo

Cada módulo de negócio SHALL possuir testes unitários próprios no projeto de testes, organizados por bounded context e camada.

#### Scenario: Testes de domínio por módulo

- **WHEN** um desenvolvedor inspeciona `apps/api/__teste__/Modules/Coletas/Domain`
- **THEN** encontra testes unitários das regras de domínio de coletas

#### Scenario: Testes de casos de uso por módulo

- **WHEN** um desenvolvedor inspeciona `apps/api/__teste__/Modules/Coletas/Application`
- **THEN** encontra testes unitários de `ColetaUseCases` com dependências externas mockadas

#### Scenario: Esqueleto de testes do módulo Auth

- **WHEN** um desenvolvedor inspeciona `apps/api/__teste__/Modules/Auth`
- **THEN** encontra estrutura preparada para testes de casos de uso de autenticação na change seguinte

#### Scenario: Casos de uso testados sem infraestrutura real

- **WHEN** testes unitários de Application de um módulo são executados
- **THEN** eles não dependem de banco de dados, HTTP ou DbContext real
