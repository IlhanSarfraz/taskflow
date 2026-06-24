export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  memberSinceUtc: string;
  tasksAssigned: number;
  tasksCompleted: number;
  tasksInProgress: number;
  projectCount: number;
}