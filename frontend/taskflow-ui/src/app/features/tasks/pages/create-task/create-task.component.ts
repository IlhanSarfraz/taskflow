import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskService } from '../../services/task.service';
import { ActivatedRoute, destroyDetachedRouteHandle, Router } from '@angular/router';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-create-task',
  imports: [ReactiveFormsModule],
  standalone: true,
  templateUrl: './create-task.component.html',
  styleUrl: './create-task.component.scss',
})
export class CreateTaskComponent {
  private fb = inject(FormBuilder);
  private taskService = inject(TaskService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = false;

  boardId!: string;
  projectId!: string;
  columnId!: string;

  form = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    priority: [1, Validators.required],
    dueDate: ['']
  });

    ngOnInit(): void {
    this.boardId = this.route.snapshot.paramMap.get('boardId')!;
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.columnId = this.route.snapshot.paramMap.get('columnId')!;
  }
  
  submit(): void {
    if(this.form.invalid) return;

    this.loading = true;

    const formValue = this.form.value;

    const request = {
      title: formValue.title ?? '',
      description: formValue.description ?? '',
      priority: Number(formValue.priority),
      dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : null,
      projectId: this.projectId,
      boardColumnId: this.columnId
    };

     this.taskService.CreateTask(request as any)
      .subscribe({
        next: () => {
          this.loading = false;

          this.router.navigate([
            '/boards',
            this.boardId
          ]);
        },
        error: (err) => {
          console.error(err);
          this.loading = false;
        }
      });
  }
}