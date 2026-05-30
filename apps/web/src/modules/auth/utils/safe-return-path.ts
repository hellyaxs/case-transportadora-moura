export function getSafeReturnPath(candidate: string | undefined | null): string | undefined {
  if (!candidate) {
    return undefined;
  }

  const trimmed = candidate.trim();

  if (!trimmed.startsWith("/")) {
    return undefined;
  }

  if (trimmed.startsWith("//")) {
    return undefined;
  }

  if (/^[a-zA-Z][a-zA-Z\d+\-.]*:/.test(trimmed)) {
    return undefined;
  }

  if (trimmed === "/login" || trimmed.startsWith("/login?") || trimmed.startsWith("/login#")) {
    return undefined;
  }

  return trimmed;
}

export function resolvePostLoginPath(returnPath: string | undefined): "/" | (string & {}) {
  return getSafeReturnPath(returnPath) ?? "/";
}
