import type {
  CancelCollectionRequest,
  CollectionDetailsDto,
  CreateCollectionRequest,
  OptionDto,
  PaginatedCollectionResponseDto,
  RegisterIncidentRequest,
  VehicleOptionDto,
} from "generated-api-types";
import { env } from "@transportadora-moura/env/web";

import type { CollectionFilters } from "../types/collection-filters";

const API_BASE_URL = env.VITE_API_BASE_URL;

export interface CollectionListParams extends CollectionFilters {
  page?: number;
  pageSize?: number;
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      credentials: "include",
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

async function requestWithoutBody(path: string, init?: RequestInit): Promise<void> {
  let response: Response;

  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      credentials: "include",
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
}

function toQueryString(params: CollectionListParams): string {
  const searchParams = new URLSearchParams();

  if (params.status) searchParams.set("status", params.status);
  if (params.customerId) searchParams.set("customerId", params.customerId);
  if (params.startDate) searchParams.set("startDate", params.startDate);
  if (params.endDate) searchParams.set("endDate", params.endDate);
  searchParams.set("page", String(params.page ?? 1));
  searchParams.set("pageSize", String(params.pageSize ?? 10));

  const query = searchParams.toString();
  return query ? `?${query}` : "";
}

export const collectionsService = {
  list(params: CollectionListParams) {
    return request<PaginatedCollectionResponseDto>(`/api/collections${toQueryString(params)}`);
  },

  create(payload: CreateCollectionRequest) {
    return request<CollectionDetailsDto>("/api/collections", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  start(id: string) {
    return request<CollectionDetailsDto>(`/api/collections/${id}/start`, { method: "POST" });
  },

  complete(id: string) {
    return request<CollectionDetailsDto>(`/api/collections/${id}/complete`, { method: "POST" });
  },

  cancel(id: string, payload: CancelCollectionRequest) {
    return request<CollectionDetailsDto>(`/api/collections/${id}/cancel`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  registerIncident(id: string, payload: RegisterIncidentRequest) {
    return request<CollectionDetailsDto>(`/api/collections/${id}/incidents`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  remove(id: string) {
    return requestWithoutBody(`/api/collections/${id}`, { method: "DELETE" });
  },

  listCustomers() {
    return request<OptionDto[]>("/api/customers");
  },

  listDrivers() {
    return request<OptionDto[]>("/api/drivers");
  },

  listVehicles() {
    return request<VehicleOptionDto[]>("/api/vehicles");
  },
};
