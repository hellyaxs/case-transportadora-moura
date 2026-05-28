import type { ColetaStatus } from "generated-api-types";

export interface ColetaFilters {
  status?: ColetaStatus | "";
  clienteId?: string;
  dataInicial?: string;
  dataFinal?: string;
}
