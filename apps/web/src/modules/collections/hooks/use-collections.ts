import type {
  CollectionSummaryDto,
  CreateCollectionRequest,
  OptionDto,
  VehicleOptionDto,
} from "generated-api-types";
import { useCallback, useEffect, useMemo, useState } from "react";

import { collectionsService } from "../services/collections.service";
import type { CollectionFilters } from "../types/collection-filters";

export function useCollections() {
  const [collections, setCollections] = useState<CollectionSummaryDto[]>([]);
  const [customers, setCustomers] = useState<OptionDto[]>([]);
  const [drivers, setDrivers] = useState<OptionDto[]>([]);
  const [vehicles, setVehicles] = useState<VehicleOptionDto[]>([]);
  const [filters, setFilters] = useState<CollectionFilters>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadCollections = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      setCollections(await collectionsService.list(filters));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar coletas.");
    } finally {
      setLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    void loadCollections();
  }, [loadCollections]);

  useEffect(() => {
    async function loadCatalogs() {
      try {
        const [customersResult, driversResult, vehiclesResult] = await Promise.all([
          collectionsService.listCustomers(),
          collectionsService.listDrivers(),
          collectionsService.listVehicles(),
        ]);

        setCustomers(customersResult);
        setDrivers(driversResult);
        setVehicles(vehiclesResult);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Erro ao carregar cadastros operacionais.");
      }
    }

    void loadCatalogs();
  }, []);

  const metrics = useMemo(
    () => ({
      abertas: collections.filter((collection) => collection.status === "Open").length,
      emColeta: collections.filter((collection) => collection.status === "InProgress").length,
      atrasadas: collections.filter((collection) => collection.overdue).length,
      altaPrioridade: collections.filter((collection) => collection.priority === "High").length,
    }),
    [collections],
  );

  async function execute(operation: () => Promise<unknown>) {
    setError(null);

    try {
      await operation();
      await loadCollections();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao executar operação.");
    }
  }

  return {
    collections,
    customers,
    drivers,
    vehicles,
    filters,
    setFilters,
    loading,
    error,
    metrics,
    create: (payload: CreateCollectionRequest) => execute(() => collectionsService.create(payload)),
    assign: (id: string, driverId: string, vehicleId: string) =>
      execute(() => collectionsService.assign(id, { driverId, vehicleId })),
    start: (id: string) => execute(() => collectionsService.start(id)),
    complete: (id: string) => execute(() => collectionsService.complete(id)),
    cancel: (id: string, reason?: string) => execute(() => collectionsService.cancel(id, { reason })),
    registerIncident: (id: string, description: string) =>
      execute(() => collectionsService.registerIncident(id, { description })),
    reload: loadCollections,
  };
}
