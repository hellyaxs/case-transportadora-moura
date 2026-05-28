# Infra Docker Postgres

## Purpose

Definir a infraestrutura local de Docker Compose para Postgres, incluindo persistência, healthcheck e portas documentadas.

## Requirements

### Requirement: Docker Compose com Postgres

O projeto SHALL fornecer um `docker-compose.yml` na raiz para subir um serviço Postgres local usado pela API.

#### Scenario: Subir infraestrutura local

- **WHEN** um desenvolvedor executa o comando de Docker Compose documentado
- **THEN** o serviço Postgres inicia com banco, usuário, senha e porta definidos para desenvolvimento local

### Requirement: Persistência do banco local

O serviço Postgres SHALL usar volume persistente para não perder dados a cada reinício do container.

#### Scenario: Reiniciar container

- **WHEN** o container Postgres é reiniciado sem remover volumes
- **THEN** os dados previamente gravados continuam disponíveis

### Requirement: Healthcheck do Postgres

O serviço Postgres SHALL possuir healthcheck para indicar quando está pronto para receber conexões.

#### Scenario: Banco pronto

- **WHEN** o Postgres aceita conexões com as credenciais configuradas
- **THEN** o healthcheck do serviço fica saudável

### Requirement: Portas documentadas

O projeto SHALL documentar as portas locais usadas por Postgres, API e frontend.

#### Scenario: Desenvolvedor consulta documentação

- **WHEN** um desenvolvedor lê as instruções locais
- **THEN** ele identifica a porta do banco, a URL da API e a URL do frontend
