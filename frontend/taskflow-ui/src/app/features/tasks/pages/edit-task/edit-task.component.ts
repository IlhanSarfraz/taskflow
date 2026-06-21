import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';

import { TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';
import { UpdateTaskRequest } from '../../models/update-task-request';

@Component({
  selector: 'app-edit-task',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './edit-task.component.html',
})
export class EditTaskComponent {

  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);

  taskId!: string;
  pageLoading = true;
  saving = false;
  loadError = '';

  form = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    priority: [2, Validators.required],
    dueDate: ['']
  });

  ngOnInit(): void {
    this.taskId = this.route.snapshot.paramMap.get('taskId')!;

    if (!this.taskId) {
      this.loadError = 'Invalid task id.';
      this.pageLoading = false;
      return;
    }

    this.loadTask();
  }

  loadTask(): void {
    this.pageLoading = true;
    this.loadError = '';

    this.taskService.GetTaskById(this.taskId)
      .subscribe({
        next: (task) => {
          this.form.patchValue({
            title: task.title,
            description: task.description ?? '',
            priority: Number(task.priority),
            dueDate: task.dueDate
              ? task.dueDate.split('T')[0]
              : ''
          });

          this.pageLoading = false;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
          this.loadError = 'Could not load task. It may not exist or you may not have access.';
          this.pageLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  cancel(): void {
    this.router.navigate(['/tasks', this.taskId]);
  }

  save(): void {
    if (this.form.invalid) return;

    this.saving = true;

    const request: UpdateTaskRequest = {
      title: this.form.value.title ?? '',
      description: this.form.value.description ?? '',
      priority: Number(this.form.value.priority),
      dueDate: this.form.value.dueDate
        ? new Date(this.form.value.dueDate).toISOString()
        : null
    };

    this.taskService.UpdateTask(this.taskId, request)
      .subscribe({
        next: () => {
          this.saving = false;
          this.router.navigate(['/tasks', this.taskId]);
        },
        error: (err) => {
          console.error(err);
          this.saving = false;
          this.cdr.markForCheck();
        }
      });
  }
}
