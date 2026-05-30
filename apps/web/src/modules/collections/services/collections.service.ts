import type {
  AssignCollectionRequest,
  CancelCollectionRequest,
  CollectionDetailsDto,
  CollectionSummaryDto,
  CreateCollectionRequest,
  OptionDto,
  RegisterIncidentRequest,
  VehicleOptionDto,
} from "generated-api-types";
import { env } from "@transportadora-moura/env/web";

import type { CollectionFilters } from "../types/collection-filters";

const API_BASE_URL = env.VITE_API_BASE_URL;

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

function toQueryString(filters: CollectionFilters): string {
  const params = new URLSearchParams();

  if (filters.status) params.set("status", filters.status);
  if (filters.customerId) params.set("customerId", filters.customerId);
  if (filters.startDate) params.set("startDate", filters.startDate);
  if (filters.endDate) params.set("endDate", filters.endDate);

  const query = params.toString();
  return query ? `?${query}` : "";
}

export const collectionsService = {
  list(filters: CollectionFilters) {
    return request<CollectionSummaryDto[]>(`/api/collections${toQueryString(filters)}`);
  },

  create(payload: CreateCollectionRequest) {
    return request<CollectionDetailsDto>("/api/collections", {
      method: "POST",
      body: JSON.stringify(payload),
    });
  },

  assign(id: string, payload: AssignCollectionRequest) {
    return request<CollectionDetailsDto>(`/api/collections/${id}/assignment`, {
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
