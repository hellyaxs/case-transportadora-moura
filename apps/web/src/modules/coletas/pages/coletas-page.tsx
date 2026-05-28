import type { ColetaPrioridade, ColetaStatus, CriarColetaRequest } from "generated-api-types";
import type { FormEvent } from "react";
import { useState } from "react";

import { Button } from "@transportadora-moura/ui/components/button";
import { Card, CardContent, CardHeader, CardTitle } from "@transportadora-moura/ui/components/card";
import { Input } from "@transportadora-moura/ui/components/input";

import { ColetaPrioridadeBadge, ColetaStatusBadge } from "../components/coleta-status-badge";
import { useColetas } from "../hooks/use-coletas";

const statusOptions: Array<ColetaStatus | ""> = ["", "Aberta", "EmColeta", "Coletada", "Cancelada"];
const prioridadeOptions: ColetaPrioridade[] = ["Normal", "Alta"];

function todayIsoDate() {
  return new Date().toISOString().slice(0, 10);
}

export function ColetasPage() {
  const {
    coletas,
    clientes,
    motoristas,
    veiculos,
    filters,
    setFilters,
    loading,
    error,
    metrics,
    criar,
    atribuir,
    iniciar,
    concluir,
    cancelar,
    registrarOcorrencia,
  } = useColetas();
  const [form, setForm] = useState<CriarColetaRequest>({
    clienteId: "",
    remetenteNome: "",
    remetenteEndereco: "",
    destinatarioNome: "",
    destinatarioEndereco: "",
    dataPrevistaRetirada: todayIsoDate(),
    prioridade: "Normal",
    observacoes: "",
  });

  async function handleCreate(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    await criar({
      ...form,
      clienteId: form.clienteId || clientes[0]?.id || "",
      observacoes: form.observacoes || null,
    });

    setForm({
      clienteId: clientes[0]?.id ?? "",
      remetenteNome: "",
      remetenteEndereco: "",
      destinatarioNome: "",
      destinatarioEndereco: "",
      dataPrevistaRetirada: todayIsoDate(),
      prioridade: "Normal",
      observacoes: "",
    });
  }

  async function handleAtribuir(coletaId: string) {
    const motoristaId = motoristas[0]?.id;
    const veiculoId = veiculos[0]?.id;

    if (!motoristaId || !veiculoId) return;

    await atribuir(coletaId, motoristaId, veiculoId);
  }

  async function handleCancelar(coletaId: string) {
    const motivo = window.prompt("Motivo do cancelamento", "Cancelamento operacional");
    if (motivo !== null) {
      await cancelar(coletaId, motivo);
    }
  }

  async function handleOcorrencia(coletaId: string) {
    const descricao = window.prompt("Descreva a ocorrência");
    if (!descricao) return;

    const usuarioResponsavel = window.prompt("Usuário responsável", "operador.demo");
    if (!usuarioResponsavel) return;

    await registrarOcorrencia(coletaId, descricao, usuarioResponsavel);
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
                value={form.clienteId || clientes[0]?.id || ""}
                onChange={(event) => setForm((current) => ({ ...current, clienteId: event.target.value }))}
                required
              >
                {clientes.map((cliente) => (
                  <option key={cliente.id} value={cliente.id}>
                    {cliente.nome}
                  </option>
                ))}
              </select>
              <Input
                placeholder="Remetente"
                value={form.remetenteNome}
                onChange={(event) => setForm((current) => ({ ...current, remetenteNome: event.target.value }))}
                required
              />
              <Input
                placeholder="Endereço do remetente"
                value={form.remetenteEndereco}
                onChange={(event) => setForm((current) => ({ ...current, remetenteEndereco: event.target.value }))}
                required
              />
              <Input
                placeholder="Destinatário"
                value={form.destinatarioNome}
                onChange={(event) => setForm((current) => ({ ...current, destinatarioNome: event.target.value }))}
                required
              />
              <Input
                placeholder="Endereço do destinatário"
                value={form.destinatarioEndereco}
                onChange={(event) => setForm((current) => ({ ...current, destinatarioEndereco: event.target.value }))}
                required
              />
              <Input
                type="date"
                value={form.dataPrevistaRetirada}
                onChange={(event) => setForm((current) => ({ ...current, dataPrevistaRetirada: event.target.value }))}
                required
              />
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={form.prioridade ?? "Normal"}
                onChange={(event) =>
                  setForm((current) => ({ ...current, prioridade: event.target.value as ColetaPrioridade }))
                }
              >
                {prioridadeOptions.map((prioridade) => (
                  <option key={prioridade} value={prioridade}>
                    Prioridade {prioridade}
                  </option>
                ))}
              </select>
              <Input
                placeholder="Observações"
                value={form.observacoes ?? ""}
                onChange={(event) => setForm((current) => ({ ...current, observacoes: event.target.value }))}
              />
              <Button type="submit" disabled={!clientes.length}>
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
                onChange={(event) => setFilters((current) => ({ ...current, status: event.target.value as ColetaStatus | "" }))}
              >
                {statusOptions.map((status) => (
                  <option key={status || "todos"} value={status}>
                    {status || "Todos os status"}
                  </option>
                ))}
              </select>
              <select
                className="h-8 border bg-background px-2 text-sm"
                value={filters.clienteId ?? ""}
                onChange={(event) => setFilters((current) => ({ ...current, clienteId: event.target.value }))}
              >
                <option value="">Todos os clientes</option>
                {clientes.map((cliente) => (
                  <option key={cliente.id} value={cliente.id}>
                    {cliente.nome}
                  </option>
                ))}
              </select>
              <Input
                type="date"
                value={filters.dataInicial ?? ""}
                onChange={(event) => setFilters((current) => ({ ...current, dataInicial: event.target.value }))}
              />
              <Input
                type="date"
                value={filters.dataFinal ?? ""}
                onChange={(event) => setFilters((current) => ({ ...current, dataFinal: event.target.value }))}
              />
            </div>

            {loading ? <p className="text-sm text-muted-foreground">Carregando coletas...</p> : null}

            {!loading && !coletas.length ? (
              <p className="border border-dashed p-6 text-center text-sm text-muted-foreground">
                Nenhuma coleta encontrada para os filtros atuais.
              </p>
            ) : null}

            <div className="grid gap-3">
              {coletas.map((coleta) => (
                <article
                  key={coleta.id}
                  className={`grid gap-3 border p-3 ${
                    coleta.prioridade === "Alta" ? "border-red-500/50 bg-red-500/5" : "border-border"
                  }`}
                >
                  <div className="flex flex-wrap items-start justify-between gap-3">
                    <div>
                      <div className="flex flex-wrap items-center gap-2">
                        <strong>{coleta.numero}</strong>
                        <ColetaStatusBadge status={coleta.status} />
                        <ColetaPrioridadeBadge prioridade={coleta.prioridade} />
                        {coleta.atrasada ? (
                          <span className="border border-orange-500/40 bg-orange-500/10 px-2 py-1 text-[11px] text-orange-300">
                            Atrasada
                          </span>
                        ) : null}
                      </div>
                      <p className="mt-1 text-sm text-muted-foreground">
                        {coleta.clienteNome} • {coleta.remetenteNome} → {coleta.destinatarioNome}
                      </p>
                      <p className="text-xs text-muted-foreground">
                        Retirada: {coleta.dataPrevistaRetirada} • Motorista: {coleta.motoristaNome ?? "não atribuído"} •
                        Veículo: {coleta.veiculoPlaca ?? "não atribuído"}
                      </p>
                    </div>
                    <div className="flex flex-wrap gap-2">
                      <Button size="sm" variant="outline" onClick={() => void handleAtribuir(coleta.id)}>
                        Atribuir
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => void iniciar(coleta.id)}>
                        Iniciar
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => void concluir(coleta.id)}>
                        Concluir
                      </Button>
                      <Button size="sm" variant="outline" onClick={() => void handleOcorrencia(coleta.id)}>
                        Ocorrência
                      </Button>
                      <Button size="sm" variant="destructive" onClick={() => void handleCancelar(coleta.id)}>
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
