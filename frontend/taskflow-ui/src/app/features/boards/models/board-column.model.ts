import { TaskSummary } from "./task-summary.model";

export interface BoardColumn {
  id: string;
  name: string;
  order: number;
  isDoneColumn: boolean;
  tasks: TaskSummary[];
}