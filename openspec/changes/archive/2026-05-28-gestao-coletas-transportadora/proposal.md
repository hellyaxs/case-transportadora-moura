## Why

A operação de coletas hoje depende de planilhas e mensagens, o que causa perda de informação, baixa rastreabilidade e dificuldade para priorizar o atendimento. O sistema deve centralizar o registro, acompanhamento e execução das coletas para dar previsibilidade ao time operacional e transparência sobre cada etapa.

## What Changes

- Criar o domínio de solicitações de coleta com identificação única, remetente, destinatário, cliente, data prevista, prioridade, observações e status operacional.
- Criar regras de transição de status para impedir avanços inválidos, especialmente cancelamentos irreversíveis e conclusão sem motorista e veículo.
- Permitir que a roteirização atribua motorista e veículo antes da conclusão da coleta.
- Registrar ocorrências com data, hora e usuário responsável.
- Disponibilizar listagem operacional com filtros por status, cliente e período, incluindo destaque visual para prioridade alta.
- Incluir documentação Swagger, testes unitários das regras críticas, seed inicial e migrations.

## Capabilities

### New Capabilities
- `coleta-solicitacoes`: Cadastro e consulta das solicitações de coleta com dados de remetente, destinatário, cliente, prazo, prioridade e observações.
- `coleta-fluxo-operacional`: Controle de status, cancelamento, atribuição de motorista/veículo e conclusão da coleta.
- `coleta-ocorrencias`: Registro rastreável de ocorrências operacionais vinculadas a uma solicitação de coleta.
- `coleta-acompanhamento`: Listagem e filtros operacionais para gestão diária das coletas, incluindo identificação de atraso e prioridade alta.

### Modified Capabilities

Nenhuma. Não existem specs OpenSpec anteriores para este domínio.

## Impact

- Backend .NET: novas entidades, value objects, casos de uso, repositórios, migrations, seed e endpoints REST documentados no Swagger.
- Frontend React/Nx: novo módulo de coletas com listagem, filtros, formulários de criação, atribuição e ocorrência.
- Contratos de API: DTOs explícitos e tipos gerados para consumo pelo frontend.
- Testes: cobertura unitária das transições de status e validações de ocorrência; testes adicionais para Application quando houver orquestração relevante.
