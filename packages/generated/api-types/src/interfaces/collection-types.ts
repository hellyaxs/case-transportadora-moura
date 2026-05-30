export type CollectionPriority = "Normal" | "High";

export type CollectionStatus = "Open" | "InProgress" | "Collected" | "Cancelled";

export interface AssignCollectionRequest {
  driverId: string;
  vehicleId: string;
}

export interface CancelCollectionRequest {
  reason?: string | null;
}

export interface CollectionSummaryDto {
  id: string;
  number?: string | null;
  customerId: string;
  customerName?: string | null;
  senderName?: string | null;
  recipientName?: string | null;
  expectedPickupDate: string;
  priority: CollectionPriority;
  status: CollectionStatus;
  driverName?: string | null;
  vehiclePlate?: string | null;
  overdue: boolean;
}

export interface IncidentDto {
  id: string;
  description?: string | null;
  responsibleUser?: string | null;
  registeredAt: string;
}

export interface CollectionDetailsDto {
  id: string;
  number?: string | null;
  customerId: string;
  customerName?: string | null;
  senderName?: string | null;
  senderAddress?: string | null;
  recipientName?: string | null;
  recipientAddress?: string | null;
  expectedPickupDate: string;
  priority: CollectionPriority;
  notes?: string | null;
  status: CollectionStatus;
  driverId?: string | null;
  driverName?: string | null;
  vehicleId?: string | null;
  vehiclePlate?: string | null;
  createdAt: string;
  assignedAt?: string | null;
  startedAt?: string | null;
  collectedAt?: string | null;
  cancelledAt?: string | null;
  cancellationReason?: string | null;
  overdue: boolean;
  incidents: IncidentDto[];
}

export interface CreateCollectionRequest {
  customerId: string;
  senderName?: string | null;
  senderAddress?: string | null;
  recipientName?: string | null;
  recipientAddress?: string | null;
  expectedPickupDate: string;
  priority?: CollectionPriority | null;
  notes?: string | null;
}

export interface RegisterIncidentRequest {
  description: string;
}

export interface VehicleOptionDto {
  id: string;
  plate?: string | null;
  description?: string | null;
}
