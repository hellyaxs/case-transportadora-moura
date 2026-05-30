import { redirect } from "@tanstack/react-router";
import type { AuthenticatedUserDto } from "generated-api-types";

import { authService } from "../services/auth.service";
import { getSafeReturnPath } from "../utils/safe-return-path";

interface GuardLocation {
  href?: string;
  pathname?: string;
  searchStr?: string;
  hash?: string;
}

function isRedirect(error: unknown): error is ReturnType<typeof redirect> {
  return Boolean(error && typeof error === "object" && "to" in error);
}

function getAttemptedPath(location?: GuardLocation): string | undefined {
  if (!location) {
    return undefined;
  }

  if (typeof location.href === "string") {
    return getSafeReturnPath(location.href);
  }

  if (typeof location.pathname !== "string") {
    return undefined;
  }

  const search = typeof location.searchStr === "string" ? location.searchStr : "";
  const hash = typeof location.hash === "string" ? location.hash : "";

  return getSafeReturnPath(`${location.pathname}${search}${hash}`);
}

export async function requireAuthenticatedUser(
  location?: GuardLocation,
): Promise<AuthenticatedUserDto> {
  try {
    return await authService.me();
  } catch (error) {
    if (isRedirect(error)) {
      throw error;
    }

    const returnPath = getAttemptedPath(location);

    throw redirect({
      to: "/login",
      ...(returnPath ? { search: { redirect: returnPath } } : {}),
    });
  }
}
