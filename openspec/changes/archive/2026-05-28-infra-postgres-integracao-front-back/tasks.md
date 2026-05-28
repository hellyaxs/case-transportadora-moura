## 1. Infraestrutura Local

- [x] 1.1 Criar `docker-compose.yml` na raiz com serviço Postgres, porta configurável, volume persistente e healthcheck.
- [x] 1.2 Definir valores padrão de desenvolvimento para banco, usuário, senha e porta.
- [x] 1.3 Documentar comandos para subir, parar e resetar a infraestrutura local.
- [x] 1.4 Documentar portas locais esperadas para Postgres, API e frontend.

## 2. Configuração da API com Postgres

- [x] 2.1 Adicionar provider `Npgsql.EntityFrameworkCore.PostgreSQL` à API.
- [x] 2.2 Remover dependência/runtime SQLite da API quando não for mais necessária.
- [x] 2.3 Criar `apps/api/.env` e, se adequado, `apps/api/.env.example` com variáveis do Postgres e porta da API.
- [x] 2.4 Ajustar carregamento de configuração da API para ler `.env` e variáveis de ambiente.
- [x] 2.5 Montar ou resolver `ConnectionStrings__Transportadora` a partir das variáveis do Postgres.
- [x] 2.6 Ajustar CORS para permitir a origem local do frontend por configuração.
- [x] 2.7 Regenerar ou ajustar migrations para PostgreSQL.
- [x] 2.8 Validar que migrations e seed inicial rodam em um banco Postgres vazio.

## 3. Integração Frontend e Backend

- [x] 3.1 Criar ou ajustar `apps/web/.env`/`.env.example` com `VITE_API_BASE_URL`.
- [x] 3.2 Ajustar o service de coletas para usar a URL da API vinda da configuração de ambiente.
- [x] 3.3 Remover URL hardcoded de backend do código do frontend.
- [x] 3.4 Garantir que erros de comunicação com a API sejam exibidos de forma compreensível na tela.
- [x] 3.5 Validar listagem, criação e ações operacionais consumindo a API real.

## 4. Validação

- [x] 4.1 Subir Postgres via Docker Compose e verificar healthcheck.
- [x] 4.2 Rodar API apontando para Postgres e verificar Swagger.
- [x] 4.3 Rodar frontend apontando para API local.
- [x] 4.4 Executar testes backend relevantes.
- [x] 4.5 Executar check-types/build frontend relevante.
- [x] 4.6 Atualizar tipos OpenAPI se o contrato público mudar.
