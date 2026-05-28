# API Env Postgres Config

## Purpose

Definir a configuração da API para usar PostgreSQL em desenvolvimento local, com variáveis de ambiente, migrations e seed inicial.

## Requirements

### Requirement: API usa Postgres

A API SHALL usar PostgreSQL como banco de dados de desenvolvimento local.

#### Scenario: API inicia com Postgres

- **WHEN** a API é iniciada com as variáveis de banco configuradas
- **THEN** ela conecta no Postgres e aplica/usa as migrations compatíveis com PostgreSQL

### Requirement: Configuração por .env da API

A API SHALL possuir configuração de ambiente para host, porta, banco, usuário e senha do Postgres.

#### Scenario: .env da API configurado

- **WHEN** o arquivo `.env` da API contém as variáveis de banco
- **THEN** a API usa esses valores para montar ou resolver a connection string

### Requirement: Fallback controlado de desenvolvimento

A API SHALL ter fallback seguro para desenvolvimento quando uma variável opcional não estiver definida.

#### Scenario: Porta do banco não informada

- **WHEN** a porta do Postgres não é informada explicitamente
- **THEN** a API usa a porta padrão definida para desenvolvimento local

### Requirement: Migrations e seed no Postgres

As migrations e o seed inicial SHALL funcionar usando o provider PostgreSQL.

#### Scenario: Banco vazio

- **WHEN** a API aplica migrations em um banco Postgres vazio
- **THEN** as tabelas de coletas, ocorrências, clientes, motoristas e veículos são criadas e recebem seed inicial

### Requirement: Provider SQLite removido do runtime

A API MUST NOT depender de SQLite como provider de persistência runtime depois da migração para Postgres.

#### Scenario: Restaurar dependências da API

- **WHEN** as dependências do projeto da API são restauradas
- **THEN** o provider PostgreSQL está presente e o provider SQLite não é necessário para executar o backend
