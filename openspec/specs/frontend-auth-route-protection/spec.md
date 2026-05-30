# Frontend Auth Route Protection

## Purpose

Definir a proteção de rotas operacionais no frontend, redirecionando usuários sem sessão autenticada válida para `/login` antes de renderizar telas protegidas.

## Requirements

### Requirement: Rotas operacionais protegidas por sessão

O frontend SHALL validar a sessão autenticada antes de renderizar rotas operacionais protegidas.

#### Scenario: Usuário autenticado acessa rota protegida

- **WHEN** um usuário com sessão válida acessa uma rota operacional protegida
- **THEN** o frontend permite a renderização da tela solicitada

#### Scenario: Usuário anônimo acessa rota protegida

- **WHEN** um usuário sem sessão válida acessa uma rota operacional protegida
- **THEN** o frontend redireciona o usuário para `/login` antes de renderizar a tela operacional

### Requirement: Rotas protegidas não disparam chamadas operacionais sem sessão

O frontend SHALL bloquear o carregamento de dados operacionais protegidos enquanto a sessão do usuário não estiver validada.

#### Scenario: Tentativa anônima de abrir tela operacional

- **WHEN** um usuário sem sessão válida tenta abrir a tela de coletas
- **THEN** o frontend não dispara chamadas de listagem, cadastros ou ações operacionais antes do redirecionamento para login

#### Scenario: Sessão válida confirmada

- **WHEN** a sessão do usuário é confirmada para uma rota protegida
- **THEN** a tela operacional pode iniciar suas chamadas normais de carregamento

### Requirement: Login permanece público

O frontend SHALL permitir acesso à rota `/login` sem exigir sessão autenticada.

#### Scenario: Usuário anônimo abre login

- **WHEN** um usuário sem sessão válida acessa `/login`
- **THEN** o frontend exibe o formulário de login

#### Scenario: Usuário autenticado abre login

- **WHEN** um usuário com sessão válida acessa `/login`
- **THEN** o frontend redireciona o usuário para uma rota operacional apropriada

### Requirement: Destino pretendido após autenticação

O frontend SHALL preservar o destino interno pretendido quando redirecionar um usuário anônimo de uma rota protegida para `/login`.

#### Scenario: Login após redirecionamento de rota protegida

- **WHEN** um usuário anônimo tenta acessar uma rota protegida, é redirecionado para `/login` e autentica com sucesso
- **THEN** o frontend navega para o destino protegido originalmente solicitado quando esse destino for interno e válido

#### Scenario: Destino ausente ou inválido

- **WHEN** o login é concluído sem destino de retorno válido
- **THEN** o frontend navega para `/`
