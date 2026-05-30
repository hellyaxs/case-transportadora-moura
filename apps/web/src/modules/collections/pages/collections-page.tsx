import type { CollectionPriority, CollectionStatus, CreateCollectionRequest } from "generated-api-types";
import type { FormEvent } from "react";
import { useState } from "react";

import { Button } from "@transportadora-moura/ui/components/button";
import { Card, CardContent, CardHeader, CardTitle } from "@transportadora-moura/ui/components/card";
import { Input } from "@transportadora-moura/ui/components/input";

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

export function CollectionsPage() {
  const {
    collections,
    customers,
    drivers,
    vehicles,
    filters,
    setFilters,
    loading,
    error,
    metrics,
    create,
    assign,
    start,
    complete,
    cancel,
    registerIncident,
  } = useCollections();
  const [form, setForm] = useState<CreateCollectionRequest>({
    customerId: "",
    senderName: "",
    senderAddress: "",
    recipientName: "",
    recipientAddress: "",
    expectedPickupDate: todayIsoDate(),
    priority: "Normal",
    notes: "",
  });

  async function handleCreate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    await create({
      ...form,
      customerId: form.customerId || customers[0]?.id || "",
      notes: form.notes || null,
    });

    setForm({
      customerId: customers[0]?.id ?? "",
      senderName: "",
      senderAddress: "",
      recipientName: "",
      recipientAddress: "",
      expectedPickupDate: todayIsoDate(),
      priority: "Normal",
      notes: "",
    });
  }

  async function handleAssign(collectionId: string) {
    const driverId = drivers[0]?.id;
    const vehicleId = vehicles[0]?.id;

    if (!driverId || !vehicleId) return;

    await assign(collectionId, driverId, vehicleId);
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
        <MetricCard label="Abertas" value={metrics.abertas} />
        <MetricCard label="Em coleta" value={metrics.emColeta} />
        <MetricCard label="Atrasadas" value={metrics.atrasadas} />
        <MetricCard label="Alta prioridade" value={metrics.altaPrioridade} />
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
                value={form.customerId || customers[0]?.id || ""}
                onChange={(event) => setForm((current) => ({ ...current, customerId: event.target.value }))}
                required
              >
                {customers.map((customer) => (
                  <option key={customer.id} value={customer.id}>
                    {customer.name}
                  </option>
                ))}
              </select>
              <Input
                placeholder="Remetente"
                value={form.senderName ?? ""}
                onChange={(event) => setForm((current) => ({ ...current, senderName: event.target.value }))}
                required
              />
              <Input
                placeholder="Endereço do remetente"
                value={form.senderAddress ?? ""}
                onChange={(event) => setForm((current) => ({ ...current, senderAddress: event.target.value }))}
                required
              />
              <Input
                placeholder="Destinatário"
                value={form.recipientName ?? ""}
                onChange={(event) => setForm((current) => ({ ...current, recipientName: event.target.value }))}
                required
              />
              <Input
                placeholder="Endereço do destinatário"
                value={form.recipientAddress ?? ""}
                onChange={(event) => setForm((current) => ({ ...current, recipientAddress: event.target.value }))}
                required
              />
              <Input
                type="date"
                value={form.expectedPickupDate}
                onChange={(event) => setForm((current) => ({ ...current, expectedPickupDate: event.target.value }))}
                required
              />
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={form.priority ?? "Normal"}
                onChange={(event) =>
                  setForm((current) => ({ ...current, priority: event.target.value as CollectionPriority }))
                }
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
                onChange={(event) => setForm((current) => ({ ...current, notes: event.target.value }))}
              />
              <Button type="submit" disabled={!customers.length}>
                Registrar coleta
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
                    <div className="flex flex-wrap gap-2">
                      <Button size="sm" variant="outline" onClick={() => void handleAssign(collection.id)}>
                        Atribuir
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => void start(collection.id)}>
                        Iniciar
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => void complete(collection.id)}>
                        Concluir
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => void handleIncident(collection.id)}>
                        Ocorrência
                      </Button>
                      <Button size="sm" variant="destructive" onClick={() => void handleCancel(collection.id)}>
                        Cancelar
                      </Button>
                    </div>
                  </div>
                </article>
              ))}
            </div>
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
