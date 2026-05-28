## 1. Domínio e Regras

- [x] 1.1 Criar estrutura do módulo de coletas nas camadas Domain, Application, Infrastructure e Presentation.
- [x] 1.2 Implementar entidade/agregado `Coleta` com número único, cliente, remetente, destinatário, data prevista, prioridade, observações, status, motorista, veículo e ocorrências.
- [x] 1.3 Implementar value objects/enums para status, prioridade, identificação de coleta e dados de endereço/participantes quando aplicável.
- [x] 1.4 Implementar regras de transição: abrir, atribuir motorista/veículo, iniciar coleta, concluir e cancelar.
- [x] 1.5 Garantir que coleta cancelada não retorne ao fluxo ativo.
- [x] 1.6 Garantir que coleta não seja marcada como `Coletada` sem motorista e veículo vinculados.
- [x] 1.7 Implementar registro de ocorrência com descrição, data, hora e usuário responsável.

## 2. Persistência e Dados Iniciais

- [x] 2.1 Criar entidades de persistência e mapeamentos para coletas, ocorrências, clientes, motoristas e veículos.
- [x] 2.2 Criar migrations iniciais do banco de dados.
- [x] 2.3 Criar seed inicial com clientes, motoristas, veículos e coletas em diferentes status/prioridades.
- [x] 2.4 Implementar repositórios e consultas filtradas por status, cliente e período.

## 3. Casos de Uso e API

- [x] 3.1 Criar casos de uso para criar coleta, consultar detalhe, listar com filtros, atribuir motorista/veículo, iniciar, concluir, cancelar e registrar ocorrência.
- [x] 3.2 Criar DTOs explícitos de request/response sem expor entidades de domínio.
- [x] 3.3 Criar endpoints REST para todas as operações principais de coleta.
- [x] 3.4 Documentar Swagger com descrições, exemplos e respostas de erro para regras de negócio.
- [x] 3.5 Garantir geração/atualização dos tipos OpenAPI consumidos pelo frontend.

## 4. Frontend Operacional

- [x] 4.1 Criar módulo frontend de coletas seguindo a estrutura de modules.
- [x] 4.2 Implementar serviço/hook para listar coletas com filtros por status, cliente e período.
- [x] 4.3 Implementar tela de acompanhamento com dados operacionais essenciais.
- [x] 4.4 Destacar visualmente coletas de prioridade alta.
- [x] 4.5 Identificar coletas atrasadas na listagem.
- [x] 4.6 Implementar fluxos de criação, atribuição, ocorrência, cancelamento e conclusão conforme endpoints disponíveis.

## 5. Testes e Validação

- [x] 5.1 Criar testes unitários para transições válidas de status.
- [x] 5.2 Criar testes unitários para bloqueio de retorno de coleta cancelada ao fluxo ativo.
- [x] 5.3 Criar testes unitários para impedir conclusão sem motorista e veículo.
- [x] 5.4 Criar testes unitários para registro de ocorrência com data, hora e usuário responsável.
- [x] 5.5 Validar manualmente o fluxo completo com seed inicial e Swagger.
- [x] 5.6 Executar checks de build, tipos e testes relevantes do monorepo.
