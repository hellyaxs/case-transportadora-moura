import { OcorrenciaDto } from './ocorrencia-dto';

export interface ColetaDetalheDto {
  id: string;
  numero?: string;
  clienteId: string;
  clienteNome?: string;
  remetenteNome?: string;
  remetenteEndereco?: string;
  destinatarioNome?: string;
  destinatarioEndereco?: string;
  dataPrevistaRetirada: string;
  prioridade: string;
  observacoes?: string;
  status: string;
  motoristaId?: string;
  motoristaNome?: string;
  veiculoId?: string;
  veiculoPlaca?: string;
  criadaEm: string;
  atribuidaEm?: string;
  iniciadaEm?: string;
  coletadaEm?: string;
  canceladaEm?: string;
  motivoCancelamento?: string;
  atrasada: boolean;
  ocorrencias?: OcorrenciaDto[];
}
