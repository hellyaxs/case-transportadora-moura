import type { AuthenticatedUserDto } from "generated-api-types";
import { env } from "@transportadora-moura/env/web";

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
    throw new Error("Não foi possível comunicar com a API. Verifique se o backend está rodando.");
  }

  if (!response.ok) {
    const problem = await response.json().catch(() => null);
    throw new Error(problem?.detail ?? problem?.title ?? "Não foi possível concluir a operação.");
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export const authService = {
  login(email: string, password: string) {
    return request<AuthenticatedUserDto>("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
  },

  logout() {
    return request<void>("/api/auth/logout", { method: "POST" });
  },

  me() {
    return request<AuthenticatedUserDto>("/api/auth/me");
  },
};
