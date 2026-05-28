import type {
  AtribuirColetaRequest,
  CancelarColetaRequest,
  ColetaDetalheDto,
  ColetaResumoDto,
  CriarColetaRequest,
  OptionDto,
  RegistrarOcorrenciaRequest,
  VeiculoOptionDto,
} from "generated-api-types";
import { env } from "@transportadora-moura/env/web";

import type { ColetaFilters } from "../types/coleta-filters";

const API_BASE_URL = env.VITE_API_BASE_URL;

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      headers: {
        "Content-Type": "application/json",
        ...init?.headers,
      },
      ...init,
    });
  } catch {
    throw new Error("Não foi possível comunicar com a API. Verifique se o backend está rodando e se VITE_API_BASE_URL está correto.");
  }

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? problem?.title ?? "Não foi possível concluir a operação.");
  }

  return (await response.json()) as T;
}

function toQueryString(filters: ColetaFilters): string {
  const params = new URLSearchParams();

  if (filters.status) params.set("status", filters.status);
  if (filters.clienteId) params.set("clienteId", filters.clienteId);
  if (filters.dataInicial) params.set("dataInicial", filters.dataInicial);
  if (filters.dataFinal) params.set("dataFinal", filters.dataFinal);

  const query = params.toString();
  return query ? `?${query}` : "";
}

export const coletasService = {
  listar(filters: ColetaFilters) {
    return request<ColetaResumoDto[]>(`/api/coletas${toQueryString(filters)}`);
  },

  criar(payload: CriarColetaRequest) {
    return request<ColetaDetalheDto>("/api/coletas", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  atribuir(id: string, payload: AtribuirColetaRequest) {
    return request<ColetaDetalheDto>(`/api/coletas/${id}/atribuicao`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  iniciar(id: string) {
    return request<ColetaDetalheDto>(`/api/coletas/${id}/iniciar`, { method: "POST" });
  },

  concluir(id: string) {
    return request<ColetaDetalheDto>(`/api/coletas/${id}/concluir`, { method: "POST" });
  },

  cancelar(id: string, payload: CancelarColetaRequest) {
    return request<ColetaDetalheDto>(`/api/coletas/${id}/cancelar`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  registrarOcorrencia(id: string, payload: RegistrarOcorrenciaRequest) {
    return request<ColetaDetalheDto>(`/api/coletas/${id}/ocorrencias`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  listarClientes() {
    return request<OptionDto[]>("/api/clientes");
  },

  listarMotoristas() {
    return request<OptionDto[]>("/api/motoristas");
  },

  listarVeiculos() {
    return request<VeiculoOptionDto[]>("/api/veiculos");
  },
};
