## MODIFIED Requirements

### Requirement: CORS permite origem local do frontend

A API SHALL permitir requisições vindas da origem local configurada para o frontend, incluindo envio e recebimento de cookies autenticados quando aplicável.

#### Scenario: Frontend chama API local

- **WHEN** o frontend local chama um endpoint da API
- **THEN** a API aceita a requisição CORS da origem configurada

#### Scenario: Frontend autenticado envia cookie

- **WHEN** o frontend autenticado chama a API com `credentials: 'include'`
- **THEN** a API aceita a origem configurada com credenciais e processa o cookie de sessão

## ADDED Requirements

### Requirement: Frontend envia credenciais nas chamadas autenticadas

O frontend SHALL enviar cookies de sessão nas chamadas HTTP autenticadas para a API.

#### Scenario: Chamada após login

- **WHEN** o usuário autenticado executa operações no módulo de coletas
- **THEN** as requisições HTTP incluem credenciais/cookies da sessão

### Requirement: Configuração documentada de runtime

O projeto SHALL documentar quais variáveis precisam existir para front e backend se comunicarem localmente, incluindo autenticação JWT e CORS com credenciais.

#### Scenario: Desenvolvedor configura ambiente

- **WHEN** um desenvolvedor segue a documentação local
- **THEN** ele consegue configurar `VITE_API_BASE_URL`, variáveis JWT/CORS da API e credenciais seed sem alterar código-fonte
