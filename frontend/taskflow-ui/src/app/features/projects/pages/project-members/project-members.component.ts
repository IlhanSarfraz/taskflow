import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, finalize, Subject, switchMap } from 'rxjs';
import { ProjectMember } from '../../models/project-member.model';
import { ProjectMemberService } from '../../services/project-member.service';
import { InvitationService } from '../../services/invitation.service';
import { UserSearchService } from '../../../shared/services/user-search.service';
import { NotificationService } from '../../../shared/services/notification.service';
import { UserSummary } from '../../models/user-summary.model';

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
  private invitationService = inject(InvitationService);
  private userSearch = inject(UserSearchService);
  private cdr = inject(ChangeDetectorRef);
  private notification = inject(NotificationService);

  private currentUserId = localStorage.getItem('userId') ?? '';

  projectId!: string;
  members: ProjectMember[] = [];
  loading = true;
  canManageMembers = false;

  showAddForm = false;
  searchTerm = '';
  searchResults: UserSummary[] = [];
  selectedUser: UserSummary | null = null;
  newRole = 2;
  inviting = false;

  private searchSubject = new Subject<string>();

  ngOnInit() {
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.loadMembers();

    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap(term => {
          if (term.trim().length < 2) {
            this.searchResults = [];
            return [];
          }
          return this.userSearch.search(term.trim());
        })
      )
      .subscribe(results => {
        this.searchResults = results;
        this.cdr.markForCheck();
      });
  }

  loadMembers() {
    this.loading = true;
    this.memberService.getMembers(this.projectId).subscribe({
      next: (res) => {
        this.members = res;
        this.loading = false;

        const me = this.members.find(m => m.userId === this.currentUserId);
        this.canManageMembers = !!me && (me.role === 'Owner' || me.role === 'Admin');

        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load members', err);
        this.loading = false;
      }
    });
  }

  onSearchChange(value: string) {
    this.selectedUser = null;
    this.searchSubject.next(value);
  }

  selectUser(user: UserSummary) {
    this.selectedUser = user;
    this.searchTerm = `${user.fullName} (${user.email})`;
    this.searchResults = [];
  }

  invite() {
    if (!this.selectedUser) {
      this.notification.warning('No user selected', ['Please search and select a user to invite.']);
      return;
    }

    this.inviting = true;
    this.invitationService.createInvite(
      this.projectId,
      this.selectedUser.email,
      this.newRole
    )      
    .pipe(finalize(() => {
        this.inviting = false;
        this.cdr.markForCheck();
      }))
      .subscribe({
        next: () => {
          this.searchTerm = '';
          this.selectedUser = null;
          this.newRole = 2;
          this.showAddForm = false;
          this.notification.success('Invite sent');
        },
        error: (err) => {
          this.notification.error(err?.error?.message ?? 'Could not send invite');
        }
      });
  }

  remove(userId: string) {
    this.memberService.removeMember(this.projectId, userId).subscribe({
      next: () => this.loadMembers(),
      error: (err) => console.error('Remove failed', err)
    });
  }

  initials(fullName: string): string {
    if (!fullName) return '';
    return fullName.split(' ').map(n => n[0]).join('').substring(0, 2).toUpperCase();
  }
}