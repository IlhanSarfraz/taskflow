import { CommentResponse } from './comment-response';
import { TaskDetails } from './task-details';
import { ProjectMember } from '../../projects/models/project-member.model';

export interface TaskDetailPage {
  task: TaskDetails;
  comments: CommentResponse[];
  members: ProjectMember[];
}
