export interface TaskResponse {
  id: string;
  title: string;
  description?: string;
  priority: number;
  dueDate?: string;
  projectId: string;
  boardColumnId: string;
}