export interface TaskSummary {
  id: string;
  title: string;
  priority: number;
  dueDate?: string;
  assigneeInitials?: string;
}
