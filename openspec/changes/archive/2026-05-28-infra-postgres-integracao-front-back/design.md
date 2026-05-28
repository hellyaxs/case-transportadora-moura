## Context

A aplicação de coletas já possui API, migrations, seed e frontend, mas a execução local ainda está fragmentada. A API foi implementada com SQLite e connection string em `appsettings`, enquanto o frontend usa uma URL padrão hardcoded no service. Para aproximar o projeto de um ambiente real de desenvolvimento, a infraestrutura deve passar a usar Postgres em Docker Compose e configuração por variáveis de ambiente.

Essa mudança afeta backend, infraestrutura local, geração de Swagger/tipos e runtime do frontend. O objetivo é que um desenvolvedor consiga subir o banco, configurar API/frontend e validar a comunicação entre as camadas sem editar código-fonte.

## Goals / Non-Goals

**Goals:**
- Adicionar Docker Compose com serviço Postgres e volume persistente.
- Criar `.env` da API com configuração de banco e porta.
- Migrar provider EF Core de SQLite para Npgsql/PostgreSQL.
- Manter migrations e seed inicial funcionando em Postgres.
- Garantir que o frontend use `VITE_API_BASE_URL` para comunicar com a API.
- Documentar comandos e portas locais esperadas.

**Non-Goals:**
- Não conteinerizar frontend e backend nesta mudança; o Docker Compose cobre a infraestrutura de banco.
- Não criar pipeline CI/CD.
- Não implementar autenticação, proxy reverso, observabilidade ou ambiente produtivo.
- Não trocar o domínio de coletas ou regras de negócio já implementadas.

## Decisions

### Postgres via Docker Compose na raiz

O projeto terá um `docker-compose.yml` na raiz com serviço `postgres`, porta configurável e volume nomeado. O padrão local deve ser previsível, por exemplo porta host `5432` apontando para porta container `5432`.

Alternativa considerada: instalar Postgres diretamente na máquina. Isso reduz arquivos no repositório, mas prejudica reprodutibilidade e onboarding.

### API configurada por `.env`

A API terá um arquivo `.env` próprio em `apps/api/.env` com variáveis como `POSTGRES_HOST`, `POSTGRES_PORT`, `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD` e/ou `ConnectionStrings__Transportadora`. A configuração do .NET deve ler variáveis de ambiente e montar a connection string quando necessário.

Alternativa considerada: manter tudo em `appsettings.Development.json`. Isso funciona localmente, mas deixa senha/porta acopladas ao arquivo de configuração e dificulta alinhar com Docker Compose.

### Npgsql como provider EF Core

O backend deve remover dependência operacional de SQLite e usar `Npgsql.EntityFrameworkCore.PostgreSQL`, mantendo EF Core como abstração de persistência. As migrations devem ser regeneradas ou ajustadas para Postgres.

Alternativa considerada: manter SQLite para desenvolvimento e Postgres apenas em produção. Isso cria divergência de provider e aumenta risco de comportamento diferente em migrations, tipos de data e constraints.

### Frontend por variável pública do Vite

O frontend deve usar `VITE_API_BASE_URL`, preferencialmente validada pelo pacote/env local quando isso estiver alinhado ao padrão do monorepo. O service não deve depender de URL fixa que precise ser alterada no código.

Alternativa considerada: proxy do Vite. Pode ser útil depois, mas a variável pública é mais explícita e combina com execução separada de API e frontend.

### CORS explícito para desenvolvimento

A API deve permitir a origem do frontend local, por exemplo `http://localhost:5173`, usando configuração de ambiente. Evitar `AllowAnyOrigin` como padrão permanente.

Alternativa considerada: manter CORS aberto. Isso acelera protótipo, mas cria uma configuração que tende a vazar para ambientes errados.

## Risks / Trade-offs

- Postgres não disponível localmente -> Mitigar com Docker Compose documentado e healthcheck.
- `.env` com senha real commitada -> Mitigar usando credenciais locais de desenvolvimento e, se necessário, `.env.example` para valores modelo.
- Migrations geradas para SQLite ficarem incompatíveis -> Mitigar regenerando migrations para Npgsql e validando `database update`/startup.
- Frontend apontar para porta incorreta -> Mitigar com `apps/web/.env` e documentação da URL esperada.
- CORS bloquear chamadas locais -> Mitigar com variável de origem permitida e teste manual front-back.

## Migration Plan

1. Adicionar dependência Npgsql na API e remover SQLite se não for mais usado.
2. Criar `docker-compose.yml` com Postgres, porta configurável, volume e healthcheck.
3. Criar `apps/api/.env` e, se aplicável, `.env.example` com variáveis de banco/API.
4. Ajustar `Program.cs`/configuração para ler `.env` e usar Postgres.
5. Regenerar migrations para Postgres ou criar migration compatível.
6. Ajustar frontend para ler `VITE_API_BASE_URL` do ambiente.
7. Subir Postgres, rodar API, validar migrations/seed e testar chamada do frontend para backend.

Rollback: voltar provider EF para SQLite, restaurar connection string anterior e remover dependências/arquivos de infraestrutura adicionados nesta mudança.

## Open Questions

- A porta host padrão do Postgres será `5432` ou devemos usar outra, como `5433`, para evitar conflito com instalações locais?
- O `.env` da API deve ser versionado com valores de desenvolvimento ou preferimos versionar apenas `.env.example`?
- A API deve continuar migrando o banco automaticamente no startup ou isso deve virar comando explícito de desenvolvimento?
