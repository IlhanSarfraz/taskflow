import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProjectService } from '../../services/project.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UpdateProjectRequest } from '../../models/update-project-request.model';

@Component({
  selector: 'app-edit-project',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  standalone: true,
  templateUrl: './edit-project.component.html',
})
export class EditProjectComponent {
  private fb = inject(FormBuilder);
  private projectService = inject(ProjectService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  pageLoading = true;
  saving = false;
  loadError = '';
  projectId!: string;

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['', Validators.required]
  });

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('id')!;

    if (!this.projectId) {
      this.loadError = 'Invalid project id.';
      this.pageLoading = false;
      return;
    }

    this.loadProject();
  }

  loadProject(): void {
    this.pageLoading = true;
    this.loadError = '';

    this.projectService.getProjectById(this.projectId)
      .subscribe({
        next: (project) => {
          this.form.patchValue({
            name: project.name,
            description: project.description
          });
          this.pageLoading = false;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(err);
          this.loadError = 'Could not load project. It may not exist or you may not have access.';
          this.pageLoading = false;
          this.cdr.markForCheck();
        }
      });
  }

  submit(): void {
    if (this.form.invalid) return;

    this.saving = true;

    const request: UpdateProjectRequest = {
      id: this.projectId,
      name: this.form.value.name ?? '',
      description: this.form.value.description ?? ''
    };

    this.projectService.updateProject(request)
      .subscribe({
        next: () => {
          this.saving = false;
          this.router.navigate(['/projects']);
        },
        error: (err) => {
          console.error(err);
          this.saving = false;
          this.cdr.markForCheck();
        }
      });
  }
}
