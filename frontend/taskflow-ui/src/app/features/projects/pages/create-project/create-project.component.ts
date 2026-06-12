import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-project',
  imports: [ReactiveFormsModule],
  standalone: true,
  templateUrl: './create-project.component.html',
  styleUrl: './create-project.component.scss',
})
export class CreateProjectComponent {
  private fb = inject(FormBuilder);
  private projectService = inject(ProjectService);
  private router = inject(Router);

  loading = false;

  form = this.fb.group({
    name: [``, Validators.required],
    key: [``, Validators.required],
    description: [``, Validators.required]
  });

  submit(): void{
    if(this.form.invalid) return;

    this.loading = true;

    this.projectService.CreateProject(this.form.value as any)
    .subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate([`/projects`])
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      }
    });
  }
}
