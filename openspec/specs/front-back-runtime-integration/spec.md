# Front Back Runtime Integration

## Purpose

Definir a integração local entre frontend e backend real via URL de API por ambiente, CORS e documentação de runtime.

## Requirements

### Requirement: Frontend usa URL da API por ambiente

O frontend SHALL usar uma variável de ambiente pública para definir a URL base da API.

#### Scenario: URL da API definida

- **WHEN** `VITE_API_BASE_URL` está configurada no ambiente do frontend
- **THEN** as chamadas HTTP do módulo de coletas usam essa URL como base

### Requirement: Frontend consulta backend real

O módulo de coletas SHALL consumir os endpoints reais do backend em inglês (`/api/collections`, `/api/customers`, `/api/drivers`, `/api/vehicles`) para listagem, criação e ações operacionais após a sessão autenticada ser validada para rotas protegidas.

#### Scenario: Tela de acompanhamento carregada por usuário autenticado

- **WHEN** o frontend abre a tela de coletas com a API disponível e sessão autenticada válida
- **THEN** ele busca clientes, motoristas, veículos e coletas diretamente no backend usando tipos gerados com propriedades em inglês

#### Scenario: Tela de acompanhamento solicitada sem sessão

- **WHEN** o frontend tenta abrir a tela de coletas sem sessão autenticada válida
- **THEN** ele redireciona para `/login` sem depender de respostas `401 Unauthorized` das chamadas operacionais para orientar o usuário

### Requirement: Erro de comunicação visível

O frontend SHALL exibir erro compreensível quando não conseguir se comunicar com a API.

#### Scenario: API indisponível

- **WHEN** a API não está acessível pela URL configurada
- **THEN** a tela informa que não foi possível carregar dados ou executar a operação

### Requirement: CORS permite origem local do frontend

A API SHALL permitir requisições vindas da origem local configurada para o frontend, incluindo envio e recebimento de cookies autenticados quando aplicável.

#### Scenario: Frontend chama API local

- **WHEN** o frontend local chama um endpoint da API
- **THEN** a API aceita a requisição CORS da origem configurada

#### Scenario: Frontend autenticado envia cookie

- **WHEN** o frontend autenticado chama a API com `credentials: 'include'`
- **THEN** a API aceita a origem configurada com credenciais e processa o cookie de sessão

### Requirement: Frontend envia credenciais nas chamadas autenticadas

O frontend SHALL enviar cookies de sessão nas chamadas HTTP autenticadas para a API.

#### Scenario: Chamada após login

- **WHEN** o usuário autenticado executa operações no módulo de coletas
- **THEN** as requisições HTTP incluem credenciais/cookies da sessão

### Requirement: Configuração documentada de runtime

O projeto SHALL documentar quais variáveis precisam existir para front e backend se comunicarem localmente, incluindo autenticação JWT, CORS com credenciais e o mapa de rotas/contratos em inglês.

#### Scenario: Desenvolvedor configura ambiente

- **WHEN** um desenvolvedor segue a documentação local
- **THEN** ele consegue configurar `VITE_API_BASE_URL`, variáveis JWT/CORS da API, credenciais seed e rotas `/api/collections` sem alterar código-fonte
