export interface TaskDetails {
  id: string;
  title: string;
  description?: string;
  priority: number;
  dueDate?: string;
  projectId: string;
  boardColumnId: string;
  assigneeId?: string;
}