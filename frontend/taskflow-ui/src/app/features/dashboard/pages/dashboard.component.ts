import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../services/dashboard.service';
import { DashboardOverview, DueTask, ProjectProgress } from '../models/dashboard-overview.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  private readonly dashboardService = inject(DashboardService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);

  overview?: DashboardOverview;
  loading = true;

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;

    this.dashboardService.getOverview().subscribe({
      next: (data) => {
        this.overview = data;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }

  get greeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good morning';
    if (hour < 18) return 'Good afternoon';
    return 'Good evening';
  }

  dueLabel(task: DueTask): string {
    if (!task.isOverdue) return 'due today';

    const due = new Date(task.dueDate);
    const today = new Date();
    const days = Math.floor(
      (new Date(today.getFullYear(), today.getMonth(), today.getDate()).getTime() -
        new Date(due.getFullYear(), due.getMonth(), due.getDate()).getTime()) /
        (1000 * 60 * 60 * 24)
    );

    return days <= 0 ? 'overdue' : `${days}d overdue`;
  }

  goToTask(task: DueTask): void {
    this.router.navigate(['/tasks', task.id]);
  }

  goToProject(project: ProjectProgress): void {
    this.router.navigate(['/projects', project.projectId, 'boards']);
  }
}