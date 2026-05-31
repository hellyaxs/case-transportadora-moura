## ADDED Requirements

### Requirement: Ações condicionadas ao status na listagem

O frontend SHALL exibir botões de ação por coleta conforme o status retornado pela API:

- `Open`: Iniciar, Ocorrência, Cancelar
- `InProgress`: Concluir, Ocorrência, Cancelar (sem Iniciar)
- `Collected`: Excluir apenas
- `Cancelled`: nenhum botão operacional

#### Scenario: Coleta em execução oculta Iniciar

- **WHEN** uma coleta com status `InProgress` é exibida na listagem
- **THEN** o botão Iniciar não é renderizado

#### Scenario: Coleta aberta exibe Iniciar

- **WHEN** uma coleta com status `Open` é exibida na listagem
- **THEN** o botão Iniciar é exibido e o botão Concluir não é exibido

#### Scenario: Coleta coletada exibe apenas Excluir

- **WHEN** uma coleta com status `Collected` é exibida na listagem
- **THEN** apenas o botão Excluir é exibido

#### Scenario: Coleta cancelada sem ações

- **WHEN** uma coleta com status `Cancelled` é exibida na listagem
- **THEN** nenhum botão operacional é exibido

#### Scenario: Confirmação antes de excluir

- **WHEN** o operador aciona Excluir em uma coleta coletada
- **THEN** o frontend solicita confirmação antes de chamar a API de exclusão
