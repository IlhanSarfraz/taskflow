import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ProjectService } from '../../services/project.service';
import { Project } from '../../models/project.model';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ProjectRefreshService } from '../../../shared/services/project-refresh.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-project-list',
  imports: [CommonModule, RouterLink],
  standalone: true,
  templateUrl: './project-list.component.html',
})
export class ProjectListComponent {
  private readonly projectService = inject(ProjectService)
  private cdr = inject(ChangeDetectorRef);
  private router = inject(Router);
  private projectRefresh = inject(ProjectRefreshService);
  private refreshSub?: Subscription;
  
  projects: Project[] = [];
  loading = true;

  ngOnInit(): void {
    this.loadProjects();

    this.refreshSub = this.projectRefresh.refresh$
      .subscribe(() => {
        this.loadProjects();
      });
  }

  ngOnDestroy(): void {
    this.refreshSub?.unsubscribe();
  }

  loadProjects(): void {
    this.loading = true;

    this.projectService.GetProjects().subscribe({
      next: (data) => {
        this.projects = data;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(`Failed to load projects`, err);
        this.loading = false;
      }
    })
  }
  
  goToEdit(id: string): void {
    this.router.navigate(['/projects/edit', id]);
  }

  deleteProject(id: string): void {
    const confirmDelete = confirm('Are you sure you want to delete this project?');
    if (!confirmDelete) return;

    this.projectService.deleteProject(id).subscribe({
      next: () => {
        this.projects = this.projects.filter(p => p.id !== id);

        this.cdr.markForCheck();

        if (this.projects.length === 0) {
          this.loading = false;
        }
      },
      error: (err) => {
        console.error(`Delete Failed`, err);
      }
    });
  }

  viewBoards(projectId: string): void {
    this.router.navigate([
      '/projects',
      projectId,
      'boards'
    ]);
  }

  viewMembers(projectId: string): void {
    this.router.navigate([
      '/projects',
      projectId,
      'members'
    ]);
  }
}