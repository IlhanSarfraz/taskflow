import { Component, inject } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { TaskService } from '../../services/task.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-task',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-task.component.html',
  styleUrl: './edit-task.component.scss'
})
export class EditTaskComponent {

  private readonly fb = inject(FormBuilder);
  private readonly taskService = inject(TaskService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  taskId!: string;
  loading = true;

  form = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    priority: [1, Validators.required],
    dueDate: ['']
  });

  ngOnInit(): void {

    this.taskId = this.route.snapshot.paramMap.get('taskId')!;

    this.taskService.GetTaskById(this.taskId)
      .subscribe({
        next: (task) => {
          this.form.patchValue({
            title: task.title,
            description: task.description ?? '',
            priority: task.priority,
            dueDate: task.dueDate
              ? task.dueDate.split('T')[0]
              : ''
          });

          this.loading = false;
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }

  save(): void {

    if (this.form.invalid) return;

    const request = {
      title: this.form.value.title ?? '',
      description: this.form.value.description ?? '',
      priority: Number(this.form.value.priority),
      dueDate: this.form.value.dueDate
        ? new Date(this.form.value.dueDate).toISOString()
        : null
    };

    this.taskService.UpdateTask(
      this.taskId,
      request
    )
    .subscribe({
      next: () => {
        this.router.navigate([
          '/tasks',
          this.taskId
        ]);
      },
      error: console.error
    });
  }
}