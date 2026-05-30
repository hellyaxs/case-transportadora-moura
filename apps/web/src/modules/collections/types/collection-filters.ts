import type { CollectionStatus } from "generated-api-types";

export interface CollectionFilters {
  status?: CollectionStatus | "";
  customerId?: string;
  startDate?: string;
  endDate?: string;
}
