## ADDED Requirements

### Requirement: Login com credenciais válidas

O sistema SHALL autenticar usuários operacionais por e-mail e senha e, em caso de sucesso, iniciar uma sessão autenticada.

#### Scenario: Credenciais corretas

- **WHEN** um usuário informa e-mail e senha válidos cadastrados no sistema
- **THEN** a API autentica o usuário e responde com sucesso sem expor o JWT no corpo da resposta

#### Scenario: Credenciais inválidas

- **WHEN** um usuário informa e-mail ou senha incorretos
- **THEN** a API rejeita o login com erro de autenticação e não cria sessão

### Requirement: Validar requests de autenticação

O sistema SHALL validar requests de autenticação com FluentValidation antes de executar o caso de uso de login.

#### Scenario: Login com payload inválido

- **WHEN** um cliente envia login sem e-mail ou senha válidos conforme regras de validação
- **THEN** a API rejeita a requisição com erro de validação sem tentar autenticar

#### Scenario: Login com payload válido

- **WHEN** um cliente envia e-mail e senha com formato válido
- **THEN** a API prossegue para a verificação de credenciais

### Requirement: Sessão JWT em cookie HttpOnly

O sistema SHALL armazenar o JWT de sessão exclusivamente em cookie `HttpOnly` emitido pela API.

#### Scenario: Login bem-sucedido

- **WHEN** o login é concluído com sucesso
- **THEN** a API define cookie `HttpOnly` contendo o JWT assinado

#### Scenario: Frontend após login

- **WHEN** o frontend conclui login com sucesso
- **THEN** ele não persiste o token em `localStorage`, `sessionStorage` ou variáveis JavaScript acessíveis

### Requirement: Logout encerra sessão

O sistema SHALL permitir encerrar a sessão autenticada removendo o cookie de autenticação.

#### Scenario: Logout solicitado

- **WHEN** um usuário autenticado solicita logout
- **THEN** a API invalida a sessão no cliente removendo ou expirando o cookie de autenticação

### Requirement: Consultar usuário autenticado

O sistema SHALL expor endpoint para retornar os dados básicos do usuário autenticado na sessão atual.

#### Scenario: Sessão válida

- **WHEN** um cliente autenticado consulta a sessão atual
- **THEN** a API retorna identificação do usuário autenticado

#### Scenario: Sessão ausente ou inválida

- **WHEN** um cliente consulta a sessão sem cookie válido
- **THEN** a API responde com não autenticado

### Requirement: Proteger endpoints operacionais

O sistema SHALL exigir sessão autenticada válida para acessar endpoints operacionais de coletas e cadastros auxiliares.

#### Scenario: Requisição autenticada

- **WHEN** um cliente autenticado chama um endpoint operacional existente
- **THEN** a API processa a requisição normalmente

#### Scenario: Requisição anônima

- **WHEN** um cliente sem sessão válida chama um endpoint operacional existente
- **THEN** a API responde com não autorizado

### Requirement: Usuários seed para desenvolvimento

O sistema SHALL disponibilizar usuário operacional inicial via seed/migrations para facilitar execução local e demonstração.

#### Scenario: Ambiente inicializado

- **WHEN** o banco é migrado e seed aplicado em ambiente de desenvolvimento
- **THEN** existe ao menos um usuário operacional capaz de autenticar no sistema
