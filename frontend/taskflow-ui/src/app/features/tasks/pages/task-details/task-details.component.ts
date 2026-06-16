import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskDetails } from '../../models/task-details';
import { FormsModule } from '@angular/forms';
import { ProjectMemberService } from '../../../projects/services/project-member.service';
import { ProjectMember } from '../../../projects/models/project-member.model';

export interface CommentResponse {
  id: string;
  userId: string;
  userName: string;
  content: string;
  createdAt: string;
}

@Component({
  selector: 'app-task-details',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './task-details.component.html',
  styleUrl: './task-details.component.scss',
})
export class TaskDetailsComponent {

  private taskService = inject(TaskService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private memberService = inject(ProjectMemberService);

  task?: TaskDetails;
  assigneeId: string = '';
  loading = true;
  members: ProjectMember[] = [];

  // COMMENTS
  comments: CommentResponse[] = [];
  newComment = '';
  editingCommentId: string | null = null;
  editedComment = '';

  ngOnInit(): void {
    const taskId = this.route.snapshot.paramMap.get('taskId')!;

    this.taskService.GetTaskById(taskId)
      .subscribe({
        next: (task) => {
          this.task = task;
          this.loading = false;

          this.loadMembers(task.projectId);
          this.loadComments(task.id);

          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }

  goBack(): void {
    history.back();
  }

  EditTask(): void {
    if (!this.task) return;

    this.router.navigate([
      `/tasks`,
      this.task.id,
      `edit`
    ]);
  }

  DeleteTask(): void {
    if (!this.task) return;

    const confirmDelete = confirm(
      'Are you sure you want to delete this task?'
    );

    if (!confirmDelete) return;

    this.taskService.DeleteTask(this.task.id)
      .subscribe({
        next: () => history.back(),
        error: (err) => console.error(err)
      });
  }

  AssignTask(): void {
    if (!this.task || !this.assigneeId) return;

    this.taskService.AssignTask(
      this.task.id,
      this.assigneeId
    )
    .subscribe({
      next: () => {
        alert('Task assigned successfully');
        this.ReloadTask();
      },
      error: (err) => console.error(err)
    });
  }

  ReloadTask(): void {
    this.taskService.GetTaskById(this.task!.id)
      .subscribe({
        next: (task) => this.task = task
      });
  }

  loadMembers(projectId: string) {
    this.memberService.getMembers(projectId)
      .subscribe(res => this.members = res);
  }

  // =========================
  // COMMENTS
  // =========================

  loadComments(taskId: string): void {
    this.taskService.GetComments(taskId)
      .subscribe({
        next: (res) => this.comments = res,
        error: (err) => console.error(err)
      });
  }

  addComment(): void {
    if (!this.task || !this.newComment.trim()) return;

    this.taskService.CreateComment(this.task.id, {
      content: this.newComment
    }).subscribe({
      next: (res) => {
        this.comments.unshift(res);
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