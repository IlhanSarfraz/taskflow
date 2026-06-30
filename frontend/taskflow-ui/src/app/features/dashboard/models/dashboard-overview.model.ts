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

export interface ProjectActivity {
  id: string;
  action: string;
  description: string;
  actorName: string;
  projectId: string | null;
  projectName: string | null;
  boardId: string | null;
  boardName: string | null;
  createdAtUtc: string;
}

export interface DashboardOverview {
  assignedCount: number;
  dueTodayCount: number;
  overdueCount: number;
  completedThisWeekCount: number;
  dueOrOverdueTasks: DueTask[];
  projects: ProjectProgress[];
  activity: ProjectActivity[];
}