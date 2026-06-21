import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TaskDetails } from '../../models/task-details';
import { FormsModule } from '@angular/forms';
import { ProjectMember } from '../../../projects/models/project-member.model';
import { CommentResponse } from '../../models/comment-response';
import { TaskAssignee } from '../../models/task-assignee.model';

@Component({
  selector: 'app-task-details',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './task-details.component.html',
  styleUrl: './task-details.component.scss',
})
export class TaskDetailsComponent {

  private taskService = inject(TaskService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  task?: TaskDetails;
  loading = true;
  assigning = false;
  members: ProjectMember[] = [];
  selectedMemberId = '';

  comments: CommentResponse[] = [];
  newComment = '';
  editingCommentId: string | null = null;
  editedComment = '';

  ngOnInit(): void {
    const taskId = this.route.snapshot.paramMap.get('taskId')!;
    this.loadPage(taskId);
  }

  get availableMembers(): ProjectMember[] {
    const assigned = new Set(this.task?.assignees.map(a => a.userId) ?? []);
    return this.members.filter(m => !assigned.has(m.userId));
  }

  getPriorityLabel(priority: number): string {
    const labels: Record<number, string> = {
      1: 'Low', 2: 'Medium', 3: 'High', 4: 'Critical'
    };
    return labels[priority] ?? 'Medium';
  }

  getPriorityClasses(priority: number): string {
    const classes: Record<number, string> = {
      1: 'bg-green-950 text-green-400',
      2: 'bg-amber-950 text-amber-400',
      3: 'bg-red-950 text-red-400',
      4: 'bg-red-950 text-red-400'
    };
    return classes[priority] ?? 'bg-amber-950 text-amber-400';
  }

  getInitials(name: string): string {
    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0]?.toUpperCase() ?? '')
      .join('');
  }

  getRelativeTime(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins === 1 ? '' : 's'} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours === 1 ? '' : 's'} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays === 1 ? '' : 's'} ago`;
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
  }

  loadPage(taskId: string): void {
    this.loading = true;

    this.taskService.GetTaskDetailPage(taskId).subscribe({
      next: (page) => {
        this.task = page.task;
        this.comments = page.comments;
        this.members = page.members;
        this.selectedMemberId = '';
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }

  EditTask(): void {
    if (!this.task) return;
    this.router.navigate(['/tasks', this.task.id, 'edit']);
  }

  DeleteTask(): void {
    if (!this.task) return;
    if (!confirm('Are you sure you want to delete this task?')) return;

    this.taskService.DeleteTask(this.task.id)
      .subscribe({
        next: () => this.router.navigate(['/boards', this.task!.boardId]),
        error: (err) => console.error(err)
      });
  }

  addAssignee(): void {
    if (!this.task || !this.selectedMemberId) return;

    const assigneeIds = [
      ...this.task.assignees.map(a => a.userId),
      this.selectedMemberId
    ];

    this.saveAssignees(assigneeIds);
  }

  removeAssignee(userId: string): void {
    if (!this.task) return;

    const assigneeIds = this.task.assignees
      .filter(a => a.userId !== userId)
      .map(a => a.userId);

    this.saveAssignees(assigneeIds);
  }

  private saveAssignees(assigneeIds: string[]): void {
    if (!this.task) return;

    this.assigning = true;

    this.taskService.AssignTask(this.task.id, assigneeIds)
      .subscribe({
        next: () => {
          this.taskService.GetTaskById(this.task!.id).subscribe({
            next: (task) => {
              this.task = task;
              this.selectedMemberId = '';
              this.assigning = false;
              this.cdr.markForCheck();
            },
            error: () => { this.assigning = false; }
          });
        },
        error: (err) => {
          console.error(err);
          this.assigning = false;
        }
      });
  }

  addComment(): void {
    if (!this.task || !this.newComment.trim()) return;

    this.taskService.CreateComment(this.task.id, {
      content: this.newComment
    }).subscribe({
      next: (res) => {
        this.comments.push(res);
        this.newComment = '';
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  startEdit(comment: CommentResponse): void {
    this.editingCommentId = comment.id;
    this.editedComment = comment.content;
  }

  saveEdit(commentId: string): void {
    this.taskService.UpdateComment(commentId, {
      content: this.editedComment
    }).subscribe({
      next: () => {
        const c = this.comments.find(x => x.id === commentId);
        if (c) c.content = this.editedComment;
        this.editingCommentId = null;
        this.cdr.detectChanges();
      },
      error: (err) => console.error(err)
    });
  }

  deleteComment(commentId: string): void {
    if (!confirm('Delete this comment?')) return;

    this.taskService.DeleteComment(commentId)
      .subscribe({
        next: () => {
          this.comments = this.comments.filter(x => x.id !== commentId);
          this.cdr.detectChanges();
        },
        error: (err) => console.error(err)
      });
  }
}
