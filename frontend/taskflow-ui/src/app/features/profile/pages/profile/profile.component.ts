import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { ProfileService } from '../../services/profile.service';
import { UserProfile } from '../../models/user-profile.model';
import { ActivityLog } from '../../models/activity-log.model';
import { UpdateProfileRequest } from '../../models/update-profile-request';
import { ChangePasswordRequest } from '../../models/change-password-request';
import { UtcDatePipe } from '../../../../pipes/utc-date.pipe';
import { RelativeTimePipe } from '../../../../pipes/relative-time.pipe';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RelativeTimePipe, UtcDatePipe],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

  private readonly profileService = inject(ProfileService);
  private readonly cdr = inject(ChangeDetectorRef);

  // ─── State ───────────────────────────────────────────────────────────────────

  profile?: UserProfile;
  activity: ActivityLog[] = [];

  loadingProfile = true;
  loadingActivity = true;

  // ─── Account settings form ───────────────────────────────────────────────────

  firstName = '';
  lastName = '';
  email = '';

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

  loadProfile(): void {
    this.loadingProfile = true;

    this.profileService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.firstName = data.firstName;
        this.lastName = data.lastName;
        this.email = data.email;
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

    this.profileService.getActivity().subscribe({
      next: (data) => {
        this.activity = data;
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

  // ─── Account settings ────────────────────────────────────────────────────────

  saveProfile(): void {
    if (!this.firstName.trim() || !this.email.trim()) return;

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