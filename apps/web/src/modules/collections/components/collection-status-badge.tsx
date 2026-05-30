import type { CollectionPriority, CollectionStatus } from "generated-api-types";

const statusClassName: Record<CollectionStatus, string> = {
  Open: "border-blue-500/30 bg-blue-500/10 text-blue-300",
  InProgress: "border-amber-500/30 bg-amber-500/10 text-amber-300",
  Collected: "border-emerald-500/30 bg-emerald-500/10 text-emerald-300",
  Cancelled: "border-zinc-500/30 bg-zinc-500/10 text-zinc-300",
};

const statusLabel: Record<CollectionStatus, string> = {
  Open: "Aberta",
  InProgress: "Em coleta",
  Collected: "Coletada",
  Cancelled: "Cancelada",
};

const priorityLabel: Record<CollectionPriority, string> = {
  Normal: "Normal",
  High: "Alta",
};

export function CollectionStatusBadge({ status }: { status: CollectionStatus }) {
  return (
    <span className={`inline-flex border px-2 py-1 text-[11px] font-medium ${statusClassName[status]}`}>
      {statusLabel[status]}
    </span>
  );
}

export function CollectionPriorityBadge({ priority }: { priority: CollectionPriority }) {
  const className =
    priority === "High"
      ? "border-red-500/40 bg-red-500/15 text-red-300"
      : "border-zinc-500/30 bg-zinc-500/10 text-zinc-300";

  return (
    <span className={`inline-flex border px-2 py-1 text-[11px] font-medium ${className}`}>
      {priorityLabel[priority]}
    </span>
  );
}
