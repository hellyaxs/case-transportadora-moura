export interface CriarColetaRequest {
  clienteId: string;
  remetenteNome?: string;
  remetenteEndereco?: string;
  destinatarioNome?: string;
  destinatarioEndereco?: string;
  dataPrevistaRetirada: string;
  prioridade?: string;
  observacoes?: string;
}
