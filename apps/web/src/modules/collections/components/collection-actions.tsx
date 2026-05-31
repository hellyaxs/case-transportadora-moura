import type { CollectionStatus, CollectionSummaryDto } from "generated-api-types";

import { Button } from "@transportadora-moura/ui/components/button";

type CollectionActionsProps = {
  collection: CollectionSummaryDto;
  onStart: (id: string) => void;
  onComplete: (id: string) => void;
  onIncident: (id: string) => void;
  onCancel: (id: string) => void;
  onDelete: (id: string) => void;
};

export function CollectionActions({
  collection,
  onStart,
  onComplete,
  onIncident,
  onCancel,
  onDelete,
}: CollectionActionsProps) {
  return (
    <div className="flex flex-wrap gap-2">
      {renderActions(collection.status, collection.id, {
        onStart,
        onComplete,
        onIncident,
        onCancel,
        onDelete,
      })}
    </div>
  );
}

function renderActions(
  status: CollectionStatus,
  id: string,
  handlers: Omit<CollectionActionsProps, "collection">,
) {
  switch (status) {
    case "Open":
      return (
        <>
          <ActionButton label="Iniciar" onClick={() => handlers.onStart(id)} />
          <ActionButton label="Ocorrência" onClick={() => handlers.onIncident(id)} />
          <ActionButton label="Cancelar" variant="destructive" onClick={() => handlers.onCancel(id)} />
        </>
      );
    case "InProgress":
      return (
        <>
          <ActionButton label="Concluir" onClick={() => handlers.onComplete(id)} />
          <ActionButton label="Ocorrência" onClick={() => handlers.onIncident(id)} />
          <ActionButton label="Cancelar" variant="destructive" onClick={() => handlers.onCancel(id)} />
        </>
      );
    case "Collected":
      return <ActionButton label="Excluir" variant="destructive" onClick={() => handlers.onDelete(id)} />;
    case "Cancelled":
      return null;
    default:
      return null;
  }
}

function ActionButton({
  label,
  onClick,
  variant = "outline",
}: {
  label: string;
  onClick: () => void;
  variant?: "outline" | "destructive";
}) {
  return (
    <Button size="sm" variant={variant} onClick={onClick}>
      {label}
    </Button>
  );
}
