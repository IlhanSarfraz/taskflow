import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskDetails } from '../../models/task-details';

@Component({
  selector: 'app-task-details',
  imports: [CommonModule],
  standalone: true,
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

  ngOnInit(): void{
    const taskId = this.route.snapshot.paramMap.get(`taskId`)!;

    this.taskService.GetTaskById(taskId)
    .subscribe({
      next: (task) => {
        this.task = task;
        this.loading = false;
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

  editTask(): void {
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
}
