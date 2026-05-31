import type { CollectionPriority, CollectionStatus, CreateCollectionRequest } from "generated-api-types";
import { Loader2 } from "lucide-react";
import type { FormEvent } from "react";
import { useState } from "react";

import { Button } from "@transportadora-moura/ui/components/button";
import { Card, CardContent, CardHeader, CardTitle } from "@transportadora-moura/ui/components/card";
import { Input } from "@transportadora-moura/ui/components/input";

import { CollectionActions } from "../components/collection-actions";
import { CollectionPriorityBadge, CollectionStatusBadge } from "../components/collection-status-badge";
import { useCollections } from "../hooks/use-collections";

const statusOptions: Array<CollectionStatus | ""> = ["", "Open", "InProgress", "Collected", "Cancelled"];
const statusFilterLabel: Record<CollectionStatus | "", string> = {
  "": "Todos os status",
  Open: "Aberta",
  InProgress: "Em coleta",
  Collected: "Coletada",
  Cancelled: "Cancelada",
};
const prioridadeOptions: CollectionPriority[] = ["Normal", "High"];

function todayIsoDate() {
  return new Date().toISOString().slice(0, 10);
}

function createEmptyCollectionForm(): CreateCollectionRequest {
  return {
    customerId: "",
    senderName: "",
    senderAddress: "",
    recipientName: "",
    recipientAddress: "",
    expectedPickupDate: todayIsoDate(),
    priority: "Normal",
    notes: "",
    driverId: "",
    vehicleId: "",
  };
}

export function CollectionsPage() {
  const {
    collections,
    customers,
    drivers,
    vehicles,
    filters,
    setFilters,
    page,
    totalPages,
    totalCount,
    setPage,
    loading,
    error,
    metrics,
    create,
    start,
    complete,
    cancel,
    registerIncident,
    remove,
  } = useCollections();
  const [form, setForm] = useState<CreateCollectionRequest>(createEmptyCollectionForm);
  const [isCreating, setIsCreating] = useState(false);

  function updateForm(update: Partial<CreateCollectionRequest>) {
    setForm((current: CreateCollectionRequest) => ({ ...current, ...update }));
  }

  async function handleCreate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (isCreating) return;

    setIsCreating(true);
    try {
      const created = await create({
        ...form,
        notes: form.notes || null,
      });

      if (created) {
        setForm(createEmptyCollectionForm());
      }
    } finally {
      setIsCreating(false);
    }
  }

  async function handleCancel(collectionId: string) {
    const reason = window.prompt("Motivo do cancelamento", "Cancelamento operacional");
    if (reason !== null) {
      await cancel(collectionId, reason);
    }
  }

  async function handleIncident(collectionId: string) {
    const description = window.prompt("Descreva a ocorrência");
    if (!description) return;

    await registerIncident(collectionId, description);
  }

  async function handleDelete(collectionId: string) {
    const confirmed = window.confirm("Deseja excluir permanentemente esta coleta coletada?");
    if (!confirmed) return;

    await remove(collectionId);
  }

  const canSubmit =
    customers.length > 0 &&
    drivers.length > 0 &&
    vehicles.length > 0 &&
    Boolean(form.customerId) &&
    Boolean(form.driverId) &&
    Boolean(form.vehicleId);

  return (
    <main className="container mx-auto grid max-w-7xl gap-4 px-4 py-6">
      <section className="grid gap-2">
        <p className="text-xs uppercase tracking-[0.3em] text-muted-foreground">Operação</p>
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div>
            <h1 className="text-2xl font-semibold">Gestão de Coletas</h1>
            <p className="text-sm text-muted-foreground">
              Acompanhe solicitações, prioridades, atrasos e atribuições operacionais.
            </p>
          </div>
        </div>
      </section>

      {error ? <div className="border border-destructive/40 bg-destructive/10 p-3 text-sm text-destructive">{error}</div> : null}

      <section className="grid gap-3 md:grid-cols-4">
        <MetricCard label="Abertas" value={metrics.openCount} />
        <MetricCard label="Em coleta" value={metrics.inProgressCount} />
        <MetricCard label="Atrasadas" value={metrics.overdueCount} />
        <MetricCard label="Alta prioridade" value={metrics.highPriorityCount} />
      </section>

      <section className="grid gap-4 lg:grid-cols-[360px_1fr]">
        <Card>
          <CardHeader>
            <CardTitle>Nova coleta</CardTitle>
          </CardHeader>
          <CardContent>
            <form className="grid gap-3" onSubmit={handleCreate}>
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={form.customerId}
                onChange={(event) => updateForm({ customerId: event.target.value })}
                disabled={isCreating}
                required
              >
                <option value="" disabled>
                  Selecione o cliente
                </option>
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={form.driverId}
                onChange={(event) => updateForm({ driverId: event.target.value })}
                disabled={isCreating}
                required
              >
                <option value="" disabled>
                  Selecione o motorista
                </option>
                {drivers.map((driver) => (
                  <option key={driver.id} value={driver.id}>
                    {driver.name}
                  </option>
                ))}
              </select>
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={form.vehicleId}
                onChange={(event) => updateForm({ vehicleId: event.target.value })}
                disabled={isCreating}
                required
              >
                <option value="" disabled>
                  Selecione o veículo
                </option>
                {vehicles.map((vehicle) => (
                  <option key={vehicle.id} value={vehicle.id}>
                    {vehicle.plate} • {vehicle.description}
                  </option>
                ))}
              </select>
              <Input
                placeholder="Remetente"
                value={form.senderName ?? ""}
                onChange={(event) => updateForm({ senderName: event.target.value })}
                disabled={isCreating}
                required
              />
              <Input
                placeholder="Endereço do remetente"
                value={form.senderAddress ?? ""}
                onChange={(event) => updateForm({ senderAddress: event.target.value })}
                disabled={isCreating}
                required
              />
              <Input
                placeholder="Destinatário"
                value={form.recipientName ?? ""}
                onChange={(event) => updateForm({ recipientName: event.target.value })}
                disabled={isCreating}
                required
              />
              <Input
                placeholder="Endereço do destinatário"
                value={form.recipientAddress ?? ""}
                onChange={(event) => updateForm({ recipientAddress: event.target.value })}
                disabled={isCreating}
                required
              />
              <Input
                type="date"
                value={form.expectedPickupDate}
                onChange={(event) => updateForm({ expectedPickupDate: event.target.value })}
                disabled={isCreating}
                required
              />
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={form.priority ?? "Normal"}
                onChange={(event) => updateForm({ priority: event.target.value as CollectionPriority })}
                disabled={isCreating}
              >
                {prioridadeOptions.map((priority) => (
                  <option key={priority} value={priority}>
                    Prioridade {priority === "High" ? "Alta" : "Normal"}
                  </option>
                ))}
              </select>
              <Input
                placeholder="Observações"
                value={form.notes ?? ""}
                onChange={(event) => updateForm({ notes: event.target.value })}
                disabled={isCreating}
              />
              <Button type="submit" disabled={!canSubmit || isCreating} aria-busy={isCreating}>
                {isCreating ? <Loader2 className="size-3 animate-spin" aria-hidden="true" /> : null}
                {isCreating ? "Registrando..." : "Registrar coleta"}
              </Button>
            </form>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Acompanhamento</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-4">
            <div className="grid gap-2 md:grid-cols-4">
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={filters.status ?? ""}
                onChange={(event) =>
                  setFilters((current) => ({ ...current, status: event.target.value as CollectionStatus | "" }))
                }
              >
                {statusOptions.map((status) => (
                  <option key={status || "todos"} value={status}>
                    {statusFilterLabel[status]}
                  </option>
                ))}
              </select>
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={filters.customerId ?? ""}
                onChange={(event) => setFilters((current) => ({ ...current, customerId: event.target.value }))}
              >
                <option value="">Todos os clientes</option>
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
              <Input
                type="date"
                value={filters.startDate ?? ""}
                onChange={(event) => setFilters((current) => ({ ...current, startDate: event.target.value }))}
              />
              <Input
                type="date"
                value={filters.endDate ?? ""}
                onChange={(event) => setFilters((current) => ({ ...current, endDate: event.target.value }))}
              />
            </div>

            {loading ? <p className="text-sm text-muted-foreground">Carregando coletas...</p> : null}

            {!loading && !collections.length ? (
              <p className="border border-dashed p-6 text-center text-sm text-muted-foreground">
                Nenhuma coleta encontrada para os filtros atuais.
              </p>
            ) : null}

            <div className="grid gap-3">
              {collections.map((collection) => (
                <article
                  key={collection.id}
                  className={`grid gap-3 border p-3 ${
                    collection.priority === "High" ? "border-red-500/50 bg-red-500/5" : "border-border"
                  }`}
                >
                  <div className="flex flex-wrap items-start justify-between gap-3">
                    <div>
                      <div className="flex flex-wrap items-center gap-2">
                        <strong>{collection.number}</strong>
                        <CollectionStatusBadge status={collection.status} />
                        <CollectionPriorityBadge priority={collection.priority} />
                        {collection.overdue ? (
                          <span className="border border-orange-500/40 bg-orange-500/10 px-2 py-1 text-[11px] text-orange-300">
                            Atrasada
                          </span>
                        ) : null}
                      </div>
                      <p className="mt-1 text-sm text-muted-foreground">
                        {collection.customerName} • {collection.senderName} → {collection.recipientName}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        Retirada: {collection.expectedPickupDate} • Motorista: {collection.driverName ?? "não atribuído"} •
                        Veículo: {collection.vehiclePlate ?? "não atribuído"}
                      </p>
                    </div>
                    <CollectionActions
                      collection={collection}
                      onStart={(id) => void start(id)}
                      onComplete={(id) => void complete(id)}
                      onIncident={(id) => void handleIncident(id)}
                      onCancel={(id) => void handleCancel(id)}
                      onDelete={(id) => void handleDelete(id)}
                    />
                  </div>
                </article>
              ))}
            </div>

            {totalCount > 0 ? (
              <div className="flex flex-wrap items-center justify-between gap-3 border-t pt-3">
                <p className="text-sm text-muted-foreground">
                  Página {page} de {Math.max(totalPages, 1)} • {totalCount} coleta(s)
                </p>
                <div className="flex gap-2">
                  <Button size="sm" variant="outline" disabled={page <= 1 || loading} onClick={() => setPage(page - 1)}>
                    Anterior
                  </Button>
                  <Button
                    size="sm"
                    variant="outline"
                    disabled={page >= totalPages || loading}
                    onClick={() => setPage(page + 1)}
                  >
                    Próxima
                  </Button>
                </div>
              </div>
            ) : null}
          </CardContent>
        </Card>
      </section>
    </main>
  );
}

function MetricCard({ label, value }: { label: string; value: number }) {
  return (
    <Card size="sm">
      <CardContent>
        <p className="text-xs text-muted-foreground">{label}</p>
        <strong className="text-2xl">{value}</strong>
      </CardContent>
    </Card>
  );
}
