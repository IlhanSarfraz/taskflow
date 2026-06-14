import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskDetails } from '../../models/task-details';
import { FormsModule } from '@angular/forms';
import { ProjectMemberService } from '../../../projects/services/project-member.service';
import { ProjectMember } from '../../../projects/models/project-member.model';

@Component({
  selector: 'app-task-details',
  imports: [CommonModule, FormsModule],
  standalone: true,
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

ngOnInit(): void {
  const taskId = this.route.snapshot.paramMap.get(`taskId`)!;

  this.taskService.GetTaskById(taskId)
    .subscribe({
      next: (task) => {
        this.task = task;
        this.loading = false;

        this.loadMembers(task.projectId);

        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
}

  goBack(): void{
    history.back();
  }

  EditTask(): void {
    if(!this.task) return;

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
        next: () => {
          history.back(); 
        },
        error: (err) => {
          console.error('DELETE TASK ERROR:', err);
        }
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

        // refresh task details
        this.ReloadTask();
      },
      error: (err) => {
        console.error('ASSIGN ERROR:', err);
      }
    });
  }

  ReloadTask(): void {
    this.taskService.GetTaskById(this.task?.id!)
      .subscribe({
        next: (task) => {
          this.task = task;
        }
      });
  }

  loadMembers(projectId: string) {
  this.memberService.getMembers(projectId)
    .subscribe(res =>{
      this.members = res;
    }); 
  }
}
