export interface ColetaResumoDto {
  id: string;
  numero?: string;
  clienteId: string;
  clienteNome?: string;
  remetenteNome?: string;
  destinatarioNome?: string;
  dataPrevistaRetirada: string;
  prioridade: string;
  status: string;
  motoristaNome?: string;
  veiculoPlaca?: string;
  atrasada: boolean;
}
