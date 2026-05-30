## MODIFIED Requirements

### Requirement: Frontend consulta backend real

O módulo de coletas SHALL consumir endpoints em inglês (`/api/collections`, `/api/customers`, `/api/drivers`, `/api/vehicles`) e tipos gerados com propriedades em inglês.

#### Scenario: Tela de acompanhamento carregada

- **WHEN** o frontend abre a tela de coletas com a API disponível
- **THEN** ele busca dados nos endpoints ingleses usando tipos gerados atualizados

### Requirement: Configuração documentada de runtime

O projeto SHALL documentar o mapa de rotas/contratos em inglês e manter `VITE_API_BASE_URL` inalterada em propósito.

#### Scenario: Desenvolvedor configura ambiente

- **WHEN** um desenvolvedor segue a documentação local
- **THEN** encontra glossário PT→EN e rotas `/api/collections` sem alterar variáveis de ambiente além das já existentes
