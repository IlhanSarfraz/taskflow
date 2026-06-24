import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ApiService } from '../../../core/services/api.service';
import { UserProfile } from '../models/user-profile.model';
import { ActivityLog } from '../models/activity-log.model';
import { UpdateProfileRequest } from '../models/update-profile-request';
import { ChangePasswordRequest } from '../models/change-password-request';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  private readonly api = inject(ApiService);

  getProfile(): Observable<UserProfile> {
    return this.api.get<UserProfile>(
      'users/profile'
    );
  }

  getActivity(
    page = 1,
    pageSize = 10
  ): Observable<ActivityLog[]> {
    return this.api.get<ActivityLog[]>(
      `users/activity?page=${page}&pageSize=${pageSize}`
    );
  }

    updateProfile(request: UpdateProfileRequest): Observable<void> {
    return this.api.put<void>(
        'users/profile',
        request
    );
    }

    changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.api.put<void>(
        'auth/change-password',
        request
    );
    }
}