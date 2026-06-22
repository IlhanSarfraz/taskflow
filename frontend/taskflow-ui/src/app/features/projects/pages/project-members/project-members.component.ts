import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProjectMember } from '../../models/project-member.model';
import { ProjectMemberService } from '../../services/project-member.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-project-members',
  imports: [CommonModule, RouterLink, FormsModule],
  standalone: true,
  templateUrl: './project-members.component.html',
  styleUrl: './project-members.component.scss',
})
export class ProjectMembersComponent {
  private route = inject(ActivatedRoute);
  private memberService = inject(ProjectMemberService);
  private cdr = inject(ChangeDetectorRef);
  private notification = inject(NotificationService);

  projectId!: string;
  members: ProjectMember[] = [];
  loading = true;

  showAddForm = false;
  newUserId = '';
  newRole = 2;
  adding = false;
  
  ngOnInit() {
    this.projectId = this.route.snapshot.paramMap.get(`projectId`)!;
    this.loadMembers();
  }

  loadMembers() {
    this.loading = true;
    this.memberService.getMembers(this.projectId)
      .subscribe({
        next: (res) => {
          this.members = res;
          this.loading = false;
          this.cdr.markForCheck();
        },
        error: (err) => {
          console.error(`Failed to load members`, err);
          this.loading = false;
        }
      });
  }

  add() {
    const trimmed = this.newUserId.trim();

    if (!trimmed) {
      this.notification.warning('Missing user ID', ['Please enter a user ID before adding.']);
      return;
    }

    const guidPattern = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;
    if (!guidPattern.test(trimmed)) {
      this.notification.warning('Invalid user ID', ['User ID must be a valid GUID.']);
      return;
    }

    this.adding = true;
    this.memberService.addMember(this.projectId, trimmed, this.newRole).subscribe({
      next: () => {
        this.adding = false;
        this.newUserId = '';
        this.newRole = 2;
        this.showAddForm = false;
        this.notification.success('Member added');
        this.loadMembers();
      },
      error: (err: HttpErrorResponse) => {
        this.adding = false;
        this.cdr.markForCheck();
        this.notification.error('Could not add member');
      }
    });
  }

  remove(userId: string) {
    this.memberService.removeMember(this.projectId, userId)
      .subscribe({
        next: () => this.loadMembers(),
        error: (err) => console.error(`Remove failed`, err)
      });
  }

  initials(fullName: string): string {
    if (!fullName) return '';
    return fullName
      .split(' ')
      .map(n => n[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();
  }
}