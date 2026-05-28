import type { ColetaPrioridade, ColetaStatus } from "generated-api-types";

const statusClassName: Record<ColetaStatus, string> = {
  Aberta: "border-blue-500/30 bg-blue-500/10 text-blue-300",
  EmColeta: "border-amber-500/30 bg-amber-500/10 text-amber-300",
  Coletada: "border-emerald-500/30 bg-emerald-500/10 text-emerald-300",
  Cancelada: "border-zinc-500/30 bg-zinc-500/10 text-zinc-300",
};

export function ColetaStatusBadge({ status }: { status: ColetaStatus }) {
  return (
    <span className={`inline-flex border px-2 py-1 text-[11px] font-medium ${statusClassName[status]}`}>
      {status}
    </span>
  );
}

export function ColetaPrioridadeBadge({ prioridade }: { prioridade: ColetaPrioridade }) {
  const className =
    prioridade === "Alta"
      ? "border-red-500/40 bg-red-500/15 text-red-300"
      : "border-zinc-500/30 bg-zinc-500/10 text-zinc-300";

  return <span className={`inline-flex border px-2 py-1 text-[11px] font-medium ${className}`}>{prioridade}</span>;
}
