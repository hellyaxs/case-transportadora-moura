## Why

O projeto já possui domínio, API e tela de coletas, mas a infraestrutura ainda não reflete o ambiente esperado: a API está configurada para SQLite local e o frontend depende de uma URL fixa para tentar falar com o backend. Precisamos padronizar a execução local com Docker Compose, Postgres e variáveis de ambiente para que front, backend e banco subam de forma previsível.

## What Changes

- Adicionar `docker-compose.yml` para infraestrutura local com Postgres.
- Criar configuração `.env` da API com host, porta, usuário, senha, banco e string de conexão do Postgres.
- Migrar a API de SQLite para Postgres, mantendo EF Core, migrations e seed inicial.
- Ajustar configuração do backend para ler conexão do ambiente, com fallback seguro para desenvolvimento.
- Ajustar o frontend para ler a URL da API por variável de ambiente e chamar o backend corretamente.
- Documentar os passos de execução local e portas esperadas.

## Capabilities

### New Capabilities
- `infra-docker-postgres`: Infraestrutura local com Docker Compose e Postgres para desenvolvimento.
- `api-env-postgres-config`: Configuração da API por `.env` e connection string Postgres.
- `front-back-runtime-integration`: Comunicação runtime entre frontend e backend via variável pública de URL da API.

### Modified Capabilities

Nenhuma. Não existem specs OpenSpec arquivadas para modificar; a change complementa a implementação ativa de coletas.

## Impact

- Backend .NET: troca do provider EF Core SQLite para Npgsql/PostgreSQL, ajustes de configuração e validação de migrations.
- Infraestrutura: novo Docker Compose, arquivo `.env` da API e documentação operacional.
- Frontend React/Vite: uso de variável `VITE_API_BASE_URL` para apontar para a API local.
- Desenvolvimento local: fluxo padronizado para subir banco, API e frontend com portas conhecidas.
