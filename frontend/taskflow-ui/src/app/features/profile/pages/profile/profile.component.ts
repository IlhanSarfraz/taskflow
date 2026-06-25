import { ChangeDetectorRef, Component, HostListener, inject, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ProfileService } from '../../services/profile.service';
import { UserProfile } from '../../models/user-profile.model';
import { ActivityLog } from '../../models/activity-log.model';
import { UpdateProfileRequest } from '../../models/update-profile-request';
import { ChangePasswordRequest } from '../../models/change-password-request';
import { UtcDatePipe } from '../../../../pipes/utc-date.pipe';
import { RelativeTimePipe } from '../../../../pipes/relative-time.pipe';

interface CondensedActivityItem {
  description: string;
  action: string;
  count: number;
  latestCreatedAtUtc: string;
}

interface ActivityDayGroup {
  label: string;
  items: CondensedActivityItem[];
}

const PREVIEW_PAGE_SIZE = 10;
const FULLSCREEN_PAGE_SIZE = 20;

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RelativeTimePipe, UtcDatePipe],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit, OnDestroy {

  private readonly profileService = inject(ProfileService);
  private readonly cdr = inject(ChangeDetectorRef);

  // ─── State ───────────────────────────────────────────────────────────────────

  profile?: UserProfile;
  activity: ActivityLog[] = [];

  loadingProfile = true;
  loadingActivity = true;
  activityExpanded = false;

  // ─── Activity pagination ─────────────────────────────────────────────────────

  private activityPage = 1;
  private activityPageSize = PREVIEW_PAGE_SIZE;
  hasMoreActivity = true;
  loadingMoreActivity = false;

  // ─── Account settings form ───────────────────────────────────────────────────

  firstName = '';
  lastName = '';
  email = '';

  private originalFirstName = '';
  private originalLastName = '';
  private originalEmail = '';

  savingProfile = false;
  saveProfileSuccess = false;
  saveProfileError = '';

  // ─── Change password form ────────────────────────────────────────────────────

  currentPassword = '';
  newPassword = '';
  confirmNewPassword = '';

  savingPassword = false;
  savePasswordSuccess = false;
  savePasswordError = '';

  // ─── Lifecycle ───────────────────────────────────────────────────────────────

  ngOnInit(): void {
    this.loadProfile();
    this.loadActivity();
  }

  ngOnDestroy(): void {
    this.unlockBodyScroll();
  }

  @HostListener('window:keydown.escape')
  onEscape(): void {
    if (this.activityExpanded) {
      this.collapseActivity();
    }
  }

  loadProfile(): void {
    this.loadingProfile = true;

    this.profileService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.firstName = data.firstName;
        this.lastName = data.lastName;
        this.email = data.email;

        this.originalFirstName = data.firstName;
        this.originalLastName = data.lastName;
        this.originalEmail = data.email;
        
        this.loadingProfile = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loadingProfile = false;
        this.cdr.markForCheck();
      }
    });
  }

  loadActivity(): void {
    this.loadingActivity = true;
    this.activityPage = 1;
    this.activityPageSize = PREVIEW_PAGE_SIZE;

    this.profileService.getActivity(this.activityPage, this.activityPageSize).subscribe({
      next: (data) => {
        this.activity = data;
        this.hasMoreActivity = data.length === this.activityPageSize;
        this.loadingActivity = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loadingActivity = false;
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Fullscreen activity ─────────────────────────────────────────────────────

  expandActivity(): void {
    this.activityExpanded = true;
    this.lockBodyScroll();

    // Re-fetch from page 1 with a bigger page size for the fullscreen view.
    this.loadingActivity = true;
    this.activityPage = 1;
    this.activityPageSize = FULLSCREEN_PAGE_SIZE;

    this.profileService.getActivity(this.activityPage, this.activityPageSize).subscribe({
      next: (data) => {
        this.activity = data;
        this.hasMoreActivity = data.length === this.activityPageSize;
        this.loadingActivity = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loadingActivity = false;
        this.cdr.markForCheck();
      }
    });
  }

  collapseActivity(): void {
    this.activityExpanded = false;
    this.unlockBodyScroll();
    // Restore the lightweight preview list.
    this.loadActivity();
  }

  loadMoreActivity(): void {
    if (this.loadingMoreActivity || !this.hasMoreActivity) return;

    this.loadingMoreActivity = true;
    const nextPage = this.activityPage + 1;

    this.profileService.getActivity(nextPage, this.activityPageSize).subscribe({
      next: (data) => {
        this.activity = [...this.activity, ...data];
        this.activityPage = nextPage;
        this.hasMoreActivity = data.length === this.activityPageSize;
        this.loadingMoreActivity = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error(err);
        this.loadingMoreActivity = false;
        this.cdr.markForCheck();
      }
    });
  }

  private lockBodyScroll(): void {
    document.body.style.overflow = 'hidden';
  }

  private unlockBodyScroll(): void {
    document.body.style.overflow = '';
  }

  // ─── Computed ────────────────────────────────────────────────────────────────

  get fullName(): string {
    return this.profile
      ? `${this.profile.firstName} ${this.profile.lastName}`
      : '';
  }

  get initials(): string {
    return this.profile
      ? `${this.profile.firstName[0]}${this.profile.lastName[0]}`.toUpperCase()
      : '';
  }

  /** Activity grouped by day, with consecutive duplicate entries condensed into one row + count. */
  get groupedActivity(): ActivityDayGroup[] {
    return this.groupByDay(this.activity).map(g => ({
      label: g.label,
      items: this.condenseConsecutive(g.items)
    }));
  }

  private groupByDay(items: ActivityLog[]): { label: string; items: ActivityLog[] }[] {
    const groups: { label: string; items: ActivityLog[] }[] = [];
    const todayStr = new Date().toDateString();
    const yesterdayStr = new Date(Date.now() - 86400000).toDateString();

    for (const item of items) {
      const date = new Date(item.createdAtUtc);
      const dayStr = date.toDateString();

      let label: string;
      if (dayStr === todayStr) {
        label = 'Today';
      } else if (dayStr === yesterdayStr) {
        label = 'Yesterday';
      } else {
        label = date.toLocaleDateString(undefined, { month: 'long', day: 'numeric', year: 'numeric' });
      }

      const last = groups[groups.length - 1];
      if (last?.label === label) {
        last.items.push(item);
      } else {
        groups.push({ label, items: [item] });
      }
    }
    return groups;
  }

  /** Collapses consecutive entries with the same description into a single row with a count. */
  private condenseConsecutive(items: ActivityLog[]): CondensedActivityItem[] {
    const result: CondensedActivityItem[] = [];

    for (const item of items) {
      const last = result[result.length - 1];
      if (last && last.description === item.description) {
        last.count++;
        if (new Date(item.createdAtUtc) > new Date(last.latestCreatedAtUtc)) {
          last.latestCreatedAtUtc = item.createdAtUtc;
        }
      } else {
        result.push({
          description: item.description,
          action: item.action,
          count: 1,
          latestCreatedAtUtc: item.createdAtUtc
        });
      }
    }
    return result;
  }

  // ─── Account settings ────────────────────────────────────────────────────────

  saveProfile(): void {
  if (!this.canSaveProfile) return;

  this.savingProfile = true;
  this.saveProfileSuccess = false;
  this.saveProfileError = '';
  this.cdr.markForCheck();

  const request: UpdateProfileRequest = {
    firstName: this.firstName,
    lastName: this.lastName,
    email: this.email
  };

  this.profileService.updateProfile(request).subscribe({
      next: () => {
        this.savingProfile = false;
        this.saveProfileSuccess = true;

        if (this.profile) {
          this.profile.firstName = this.firstName;
          this.profile.lastName = this.lastName;
          this.profile.email = this.email;
        }

        // New baseline — button goes back to disabled until the next edit.
        this.originalFirstName = this.firstName;
        this.originalLastName = this.lastName;
        this.originalEmail = this.email;

        this.loadActivity();
        this.cdr.markForCheck();

        setTimeout(() => {
          this.saveProfileSuccess = false;
          this.cdr.markForCheck();
        }, 3000);
      },
      error: (err) => {
        this.savingProfile = false;
        this.saveProfileError = err?.error?.message ?? 'Failed to save changes.';
        this.cdr.markForCheck();
      }
    });
  }

  get isProfileDirty(): boolean {
    return (
      this.firstName.trim() !== this.originalFirstName.trim() ||
      this.lastName.trim() !== this.originalLastName.trim() ||
      this.email.trim() !== this.originalEmail.trim()
    );
  }

  get canSaveProfile(): boolean {
    return this.isProfileDirty && !!this.firstName.trim() && !!this.email.trim() && !this.savingProfile;
  }

  // ─── Change password ─────────────────────────────────────────────────────────

  changePassword(): void {
    if (!this.currentPassword || !this.newPassword || !this.confirmNewPassword) return;
    if (this.newPassword !== this.confirmNewPassword) {
      this.savePasswordError = 'Passwords do not match.';
      return;
    }

    this.savingPassword = true;
    this.savePasswordSuccess = false;
    this.savePasswordError = '';

    const request: ChangePasswordRequest = {
      currentPassword: this.currentPassword,
      newPassword: this.newPassword,
    };

    this.profileService.changePassword(request).subscribe({
      next: () => {
        this.savingPassword = false;
        this.savePasswordSuccess = true;

        this.currentPassword = '';
        this.newPassword = '';
        this.confirmNewPassword = '';

        this.loadActivity();
        this.cdr.markForCheck();

        setTimeout(() => {
          this.savePasswordSuccess = false;
          this.cdr.markForCheck();
        }, 3000);
      },
      error: (err) => {
        this.savingPassword = false;
        this.savePasswordError = err?.error?.message ?? 'Failed to update password.';
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Activity helpers ─────────────────────────────────────────────────────────

  getActivityDotColor(action: string): string {
    if (action.includes('Deleted') || action.includes('Declined') || action.includes('Removed')) {
      return 'bg-red-500';
    }
    if (action.includes('Updated') || action.includes('Moved') || action.includes('Changed')) {
      return 'bg-amber-400';
    }
    return 'bg-[#08CB00]';
  }
}