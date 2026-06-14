import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { TaskService } from '../../services/task.service';
import { Router } from '@angular/router';
import { TaskDetails } from '../../models/task-details';

@Component({
  selector: 'app-my-tasks',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './my-tasks.component.html',
  styleUrl: './my-tasks.component.scss',
})
export class MyTasksComponent {
  private taskService = inject(TaskService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  tasks: TaskDetails[] = [];
  loading = true;

  ngOnInit(){
    this.taskService.GetMyTasks()
      .subscribe({
        next: (res) => {
          this.tasks = res;
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }

  openTask(taskId: string){
    this.router.navigate([`/tasks`, taskId])
  }
}
