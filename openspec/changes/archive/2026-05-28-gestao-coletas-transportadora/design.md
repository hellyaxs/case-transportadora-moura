## Context

A transportadora precisa substituir um processo operacional baseado em planilhas e WhatsApp por um sistema interno rastreável. O domínio central é a solicitação de coleta, que nasce aberta, pode ser roteirizada com motorista e veículo, pode receber ocorrências e termina como coletada ou cancelada. O projeto atual combina backend .NET dentro do Nx, frontend React e geração de tipos a partir de OpenAPI.

Os principais interessados são clientes/atendentes que abrem solicitações, roteirizadores que organizam a operação, motoristas ou operadores que registram ocorrências e gestores que acompanham pendências, atrasos e prioridades.

## Goals / Non-Goals

**Goals:**
- Modelar coletas como domínio próprio, com regras críticas no Domain e orquestração no Application.
- Expor endpoints REST documentados para criação, consulta, filtros, atribuição, cancelamento, conclusão e ocorrências.
- Persistir coletas, responsáveis, veículos, motoristas e ocorrências com migrations e seed inicial.
- Disponibilizar uma tela operacional com filtros e destaque visual para prioridade alta.
- Cobrir regras de transição de status com testes unitários.

**Non-Goals:**
- Não implementar otimização automática de rotas, geolocalização em tempo real ou integração com WhatsApp.
- Não modelar financeiro, faturamento, cálculo de frete ou emissão fiscal.
- Não implementar autenticação completa nesta change; o usuário responsável por ocorrência pode ser recebido do contexto atual ou de um campo controlado enquanto autenticação não existir.

## Decisions

### Domínio de coleta como agregado principal

A entidade `Coleta` será o aggregate root do módulo de coletas. Ela concentra número único, cliente, remetente, destinatário, data prevista de retirada, prioridade, observações, status, motorista, veículo e ocorrências.

Alternativa considerada: separar status, atribuição e ocorrência em serviços independentes sem aggregate root claro. Isso deixaria as invariantes espalhadas e aumentaria o risco de permitir conclusão sem atribuição ou retorno de cancelamento ao fluxo ativo.

### Regras de transição no Domain

As operações `AtribuirMotoristaVeiculo`, `Cancelar`, `MarcarEmColeta`, `MarcarColetada` e `RegistrarOcorrencia` devem existir como métodos de domínio ou serviços de domínio puros. Controllers apenas recebem HTTP, validam entrada superficial e chamam casos de uso.

Alternativa considerada: validar transições nos endpoints ou handlers. Essa abordagem é mais rápida inicialmente, mas viola a arquitetura definida e dificulta testes unitários das regras críticas.

### Status explícito e fluxo conservador

O fluxo base será: `Aberta` -> `EmColeta` -> `Coletada`, com `Cancelada` como estado terminal a partir de estados ativos. A atribuição de motorista e veículo deve ocorrer antes de `EmColeta` ou `Coletada`; a conclusão exige ambos vinculados.

Alternativa considerada: permitir concluir diretamente de `Aberta` quando houver atribuição. Essa regra pode acelerar operação, mas reduz clareza do acompanhamento. Se a operação real exigir conclusão direta, o caso de uso deve registrar a transição intermediária de forma explícita.

### DTOs e contratos OpenAPI

O backend não deve expor entidades de domínio diretamente. Cada endpoint terá request/response DTOs explícitos, exemplos e descrições no Swagger. O frontend consumirá tipos gerados a partir do contrato OpenAPI.

Alternativa considerada: compartilhar tipos internos ou retornar entidades EF. Isso acopla persistência, domínio e UI, dificultando mudanças futuras.

### Filtros no backend, destaque visual no frontend

Filtros por status, cliente e período serão aplicados no backend para manter paginação e performance previsíveis. O frontend recebe prioridade e data prevista para destacar prioridade alta e atraso.

Alternativa considerada: carregar tudo e filtrar no cliente. Serve para protótipo, mas não escala para dezenas ou centenas de solicitações diárias.

### Persistência relacional com migrations e seed

O banco deve armazenar coletas e ocorrências com relacionamento explícito. Motoristas, veículos e clientes podem iniciar como tabelas simples com seed para viabilizar desenvolvimento, testes manuais e demonstração.

Alternativa considerada: dados em memória ou arquivo local. Isso aceleraria o início, mas não atende a rastreabilidade, migrations e seed exigidos.

## Risks / Trade-offs

- Regra de usuário responsável sem autenticação completa -> Mitigar isolando a origem do usuário em uma interface de Application, permitindo trocar depois por autenticação real.
- Crescimento da listagem operacional -> Mitigar com filtros no backend, paginação e índices por status, cliente e data prevista.
- Ambiguidade de status operacional -> Mitigar documentando transições permitidas e cobrindo cada transição crítica com testes unitários.
- Dados de motorista/veículo inicialmente simples -> Mitigar modelando IDs e nomes de forma suficiente para a operação atual, sem criar um módulo complexo de frota antes da necessidade.

## Migration Plan

1. Criar migrations para tabelas de coletas, ocorrências, clientes, motoristas e veículos.
2. Adicionar seed inicial com clientes, motoristas, veículos e coletas em diferentes status.
3. Implementar domínio e testes unitários antes dos endpoints.
4. Expor endpoints REST com Swagger documentado.
5. Gerar tipos OpenAPI e implementar módulo frontend de acompanhamento.
6. Validar fluxo manual completo: criar, atribuir, marcar em coleta, registrar ocorrência, concluir e cancelar.

Rollback: como a funcionalidade é nova, rollback consiste em remover endpoints da publicação, reverter migrations aplicadas em ambiente não produtivo e ocultar rotas do frontend.

## Open Questions

- O número único da coleta deve seguir padrão operacional específico, como prefixo por ano ou filial, ou pode iniciar como sequência controlada pelo sistema?
- Clientes, motoristas e veículos serão cadastrados neste sistema ou apenas semeados/selecionados a partir de dados existentes?
- A coleta pode ir diretamente de `Aberta` para `Coletada` se motorista e veículo já estiverem atribuídos, ou a etapa `EmColeta` é obrigatória na operação?
