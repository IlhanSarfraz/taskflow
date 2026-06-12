import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-edit-project',
  imports: [ReactiveFormsModule],
  standalone: true,
  templateUrl: './edit-project.component.html',
  styleUrl: './edit-project.component.scss',
})
export class EditProjectComponent {
  private fb = inject(FormBuilder);
  private projectService = inject(ProjectService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  loading = false;
  projectId!: string;

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.required]
  });

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get(`id`)!;
    this.loadProject();
  }

  loadProject(): void {
    this.projectService.getProjectById(this.projectId)
    .subscribe({
      next: (project) => {
        this.form.patchValue({
          name: project.name,
          description: project.description
        });
      },
      error: (err) => console.error(err)
    });
  }

  submit(): void{
    if(this.form.invalid) return;

    this.loading = true;

    this.projectService.updateProject({
      id: this.projectId,
      ...this.form.value
    } as any)
    .subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate([`/projects`]);
      }
    });
  }
}
