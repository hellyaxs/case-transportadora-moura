import type {
  CollectionListMetricsDto,
  CollectionSummaryDto,
  CreateCollectionRequest,
  OptionDto,
  VehicleOptionDto,
} from "generated-api-types";
import { useCallback, useEffect, useRef, useState } from "react";

import { collectionsService } from "../services/collections.service";
import type { CollectionFilters } from "../types/collection-filters";

const DEFAULT_PAGE_SIZE = 10;

export function useCollections() {
  const [collections, setCollections] = useState<CollectionSummaryDto[]>([]);
  const [customers, setCustomers] = useState<OptionDto[]>([]);
  const [drivers, setDrivers] = useState<OptionDto[]>([]);
  const [vehicles, setVehicles] = useState<VehicleOptionDto[]>([]);
  const [filters, setFiltersState] = useState<CollectionFilters>({});
  const [page, setPage] = useState(1);
  const [pageSize] = useState(DEFAULT_PAGE_SIZE);
  const [totalPages, setTotalPages] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [metrics, setMetrics] = useState<CollectionListMetricsDto>({
    openCount: 0,
    inProgressCount: 0,
    overdueCount: 0,
    highPriorityCount: 0,
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const filtersRef = useRef(filters);

  const setFilters = useCallback((updater: CollectionFilters | ((current: CollectionFilters) => CollectionFilters)) => {
    setFiltersState((current) => {
      const next = typeof updater === "function" ? updater(current) : updater;
      filtersRef.current = next;
      return next;
    });
    setPage(1);
  }, []);

  const loadCollections = useCallback(async (targetPage = page) => {
    setLoading(true);
    setError(null);

    try {
      const response = await collectionsService.list({
        ...filtersRef.current,
        page: targetPage,
        pageSize,
      });

      setCollections(response.items);
      setPage(response.page);
      setTotalPages(response.totalPages);
      setTotalCount(response.totalCount);
      setMetrics(response.metrics);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao carregar coletas.");
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    void loadCollections(page);
  }, [filters, page, pageSize, loadCollections]);

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

  async function execute(operation: () => Promise<unknown>) {
    setError(null);

    try {
      await operation();
      await loadCollections(page);
      return true;
    } catch (err) {
      setError(err instanceof Error ? err.message : "Erro ao executar operação.");
      return false;
    }
  }

  return {
    collections,
    customers,
    drivers,
    vehicles,
    filters,
    setFilters,
    page,
    pageSize,
    totalPages,
    totalCount,
    setPage,
    loading,
    error,
    metrics,
    create: (payload: CreateCollectionRequest) => execute(() => collectionsService.create(payload)),
    start: (id: string) => execute(() => collectionsService.start(id)),
    complete: (id: string) => execute(() => collectionsService.complete(id)),
    cancel: (id: string, reason?: string) => execute(() => collectionsService.cancel(id, { reason })),
    registerIncident: (id: string, description: string) =>
      execute(() => collectionsService.registerIncident(id, { description })),
    remove: (id: string) => execute(() => collectionsService.remove(id)),
    reload: loadCollections,
  };
}
