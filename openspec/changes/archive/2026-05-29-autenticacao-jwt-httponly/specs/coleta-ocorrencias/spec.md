## MODIFIED Requirements

### Requirement: Rastreabilidade da ocorrência

O sistema SHALL guardar data, hora e usuário responsável em toda ocorrência registrada, usando a identidade autenticada da sessão atual como origem do usuário responsável.

#### Scenario: Ocorrência criada

- **WHEN** um usuário autenticado registra uma ocorrência em uma coleta
- **THEN** o sistema salva data, hora e identificação do usuário autenticado como responsável

#### Scenario: Usuário responsável ausente

- **WHEN** a requisição de ocorrência não possui sessão autenticada válida
- **THEN** o sistema rejeita o registro antes de persistir a ocorrência

## REMOVED Requirements

### Requirement: Usuário responsável informado manualmente na requisição

**Reason**: A identificação do responsável passa a ser derivada da autenticação JWT, eliminando falsificação manual no body.

**Migration**: Remover `ResponsibleUser` de `RegisterIncidentRequest` e consumir `ICurrentUser`/claims no backend; remover campo correspondente no frontend.
