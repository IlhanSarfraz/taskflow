import { TaskAssignee } from './task-assignee.model';

export interface TaskDetails {
  id: string;
  title: string;
  description?: string;
  priority: number;
  dueDate?: string;
  projectId: string;
  projectName: string;
  boardColumnId: string;
  columnName: string;
  boardId: string;
  assignees: TaskAssignee[];
}
