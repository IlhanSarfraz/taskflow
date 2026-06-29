export interface DueTask {
  id: string;
  title: string;
  priority: number;
  dueDate: string;
  isOverdue: boolean;
  projectId: string;
  projectName: string;
  boardId: string;
  boardName: string;
}

export interface ProjectProgress {
  projectId: string;
  projectName: string;
  totalTaskCount: number;
  completedTaskCount: number;
  progressPercent: number;
}

export interface DashboardOverview {
  assignedCount: number;
  dueTodayCount: number;
  overdueCount: number;
  completedThisWeekCount: number;
  dueOrOverdueTasks: DueTask[];
  projects: ProjectProgress[];
}