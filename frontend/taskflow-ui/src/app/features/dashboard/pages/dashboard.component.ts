import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { DashboardService } from '../services/dashboard.service';
import { DashboardOverview, DueTask, ProjectActivity, ProjectProgress } from '../models/dashboard-overview.model';
import { RelativeTimePipe } from '../../../pipes/relative-time.pipe';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RelativeTimePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {

  private readonly dashboardService = inject(DashboardService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);

  overview?: DashboardOverview;
  loading = true;

  // Used to substitute "You" for the current user's name in the feed.
  private readonly currentUserName =
    `${localStorage.getItem('firstName') ?? ''} ${localStorage.getItem('lastName') ?? ''}`.trim();

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
    const firstName = localStorage.getItem('firstName') ?? '';
    const timeOfDay = hour < 12 ? 'Good morning' : hour < 18 ? 'Good afternoon' : 'Good evening';
    return firstName ? `${timeOfDay}, ${firstName}` : timeOfDay;
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

  actorLabel(item: ProjectActivity): string {
    return item.actorName === this.currentUserName ? 'You' : item.actorName;
  }

  activityDotColor(action: string): string {
    if (action.includes('Deleted') || action.includes('Removed') || action.includes('Declined'))
      return 'bg-red-500';
    if (action.includes('Updated') || action.includes('Moved') || action.includes('Changed') || action.includes('Edited'))
      return 'bg-amber-400';
    return 'bg-[#08CB00]';
  }

  goToTask(task: DueTask): void {
    this.router.navigate(['/tasks', task.id]);
  }

  goToProject(project: ProjectProgress): void {
    this.router.navigate(['/projects', project.projectId, 'boards']);
  }
}