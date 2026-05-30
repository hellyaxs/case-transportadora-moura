## MODIFIED Requirements

### Requirement: Frontend consulta backend real

O módulo de coletas SHALL consumir os endpoints reais do backend para listagem, criação e ações operacionais após a sessão autenticada ser validada para rotas protegidas.

#### Scenario: Tela de acompanhamento carregada por usuário autenticado

- **WHEN** o frontend abre a tela de coletas com a API disponível e sessão autenticada válida
- **THEN** ele busca clientes, motoristas, veículos e coletas diretamente no backend

#### Scenario: Tela de acompanhamento solicitada sem sessão

- **WHEN** o frontend tenta abrir a tela de coletas sem sessão autenticada válida
- **THEN** ele redireciona para `/login` sem depender de respostas `401 Unauthorized` das chamadas operacionais para orientar o usuário
