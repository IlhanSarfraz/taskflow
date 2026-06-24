export interface ActivityLog {
  id: string;
  action: string;
  entityType: string;
  entityId: string;
  description: string;
  createdAtUtc: string;
}