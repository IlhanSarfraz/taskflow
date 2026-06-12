export interface UpdateTaskRequest {
  title: string;
  description?: string;
  priority: number;
  dueDate?: string | null;
}