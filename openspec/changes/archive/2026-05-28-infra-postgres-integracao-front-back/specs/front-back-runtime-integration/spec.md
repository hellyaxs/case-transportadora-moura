## ADDED Requirements

### Requirement: Frontend usa URL da API por ambiente
O frontend SHALL usar uma variável de ambiente pública para definir a URL base da API.

#### Scenario: URL da API definida
- **WHEN** `VITE_API_BASE_URL` está configurada no ambiente do frontend
- **THEN** as chamadas HTTP do módulo de coletas usam essa URL como base

### Requirement: Frontend consulta backend real
O módulo de coletas SHALL consumir os endpoints reais do backend para listagem, criação e ações operacionais.

#### Scenario: Tela de acompanhamento carregada
- **WHEN** o frontend abre a tela de coletas com a API disponível
- **THEN** ele busca clientes, motoristas, veículos e coletas diretamente no backend

### Requirement: Erro de comunicação visível
O frontend SHALL exibir erro compreensível quando não conseguir se comunicar com a API.

#### Scenario: API indisponível
- **WHEN** a API não está acessível pela URL configurada
- **THEN** a tela informa que não foi possível carregar dados ou executar a operação

### Requirement: CORS permite origem local do frontend
A API SHALL permitir requisições vindas da origem local configurada para o frontend.

#### Scenario: Frontend chama API local
- **WHEN** o frontend local chama um endpoint da API
- **THEN** a API aceita a requisição CORS da origem configurada

### Requirement: Configuração documentada de runtime
O projeto SHALL documentar quais variáveis precisam existir para front e backend se comunicarem localmente.

#### Scenario: Desenvolvedor configura ambiente
- **WHEN** um desenvolvedor segue a documentação local
- **THEN** ele consegue configurar `VITE_API_BASE_URL` e as variáveis da API sem alterar código-fonte
