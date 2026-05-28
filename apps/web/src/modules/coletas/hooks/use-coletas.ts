import type {
  ColetaResumoDto,
  CriarColetaRequest,
  OptionDto,
  VeiculoOptionDto,
} from "generated-api-types";
import { useCallback, useEffect, useMemo, useState } from "react";

import { coletasService } from "../services/coletas.service";
import type { ColetaFilters } from "../types/coleta-filters";

export function useColetas() {
  const [coletas, setColetas] = useState<ColetaResumoDto[]>([]);
  const [clientes, setClientes] = useState<OptionDto[]>([]);
  const [motoristas, setMotoristas] = useState<OptionDto[]>([]);
  const [veiculos, setVeiculos] = useState<VeiculoOptionDto[]>([]);
  const [filters, setFilters] = useState<ColetaFilters>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const carregarColetas = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      setColetas(await coletasService.listar(filters));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar coletas.");
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    void carregarColetas();
  }, [carregarColetas]);

  useEffect(() => {
    async function carregarCadastros() {
      try {
        const [clientesResult, motoristasResult, veiculosResult] = await Promise.all([
          coletasService.listarClientes(),
          coletasService.listarMotoristas(),
          coletasService.listarVeiculos(),
        ]);

        setClientes(clientesResult);
        setMotoristas(motoristasResult);
        setVeiculos(veiculosResult);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao carregar cadastros operacionais.");
      }
    }

    void carregarCadastros();
  }, []);

  const metrics = useMemo(
    () => ({
      abertas: coletas.filter((coleta) => coleta.status === "Aberta").length,
      emColeta: coletas.filter((coleta) => coleta.status === "EmColeta").length,
      atrasadas: coletas.filter((coleta) => coleta.atrasada).length,
      altaPrioridade: coletas.filter((coleta) => coleta.prioridade === "Alta").length,
    }),
    [coletas],
  );

  async function executar(operacao: () => Promise<unknown>) {
    setError(null);

    try {
      await operacao();
      await carregarColetas();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao executar operação.");
    }
  }

  return {
    coletas,
    clientes,
    motoristas,
    veiculos,
    filters,
    setFilters,
    loading,
    error,
    metrics,
    criar: (payload: CriarColetaRequest) => executar(() => coletasService.criar(payload)),
    atribuir: (id: string, motoristaId: string, veiculoId: string) =>
      executar(() => coletasService.atribuir(id, { motoristaId, veiculoId })),
    iniciar: (id: string) => executar(() => coletasService.iniciar(id)),
    concluir: (id: string) => executar(() => coletasService.concluir(id)),
    cancelar: (id: string, motivo?: string) =>
      executar(() => coletasService.cancelar(id, { motivo })),
    registrarOcorrencia: (id: string, descricao: string, usuarioResponsavel: string) =>
      executar(() => coletasService.registrarOcorrencia(id, { descricao, usuarioResponsavel })),
    recarregar: carregarColetas,
  };
}
