export interface CreateTaskRequest {
  title: string;
  description?: string;
  priority: number;
  dueDate?: string;
  projectId: string;
  boardColumnId: string;
}