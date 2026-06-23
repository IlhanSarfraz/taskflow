// src/app/projects/services/invitation.service.ts
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Invite } from '../models/invite.model';
import { ApiService } from '../../../core/services/api.service';

@Injectable({
  providedIn: 'root'
})
export class InvitationService {
  private api = inject(ApiService);

  createInvite(projectId: string, invitedUserId: string, role: number): Observable<void> {
    return this.api.post<void>(
      `projects/projectId=${projectId}/invites`,
      { projectId, invitedUserId, role }
    );
  }

  getMyInvites(): Observable<Invite[]> {
    return this.api.get<Invite[]>(`invites/my`);
  }

  accept(inviteId: string): Observable<void> {
    return this.api.post<void>(`invites/${inviteId}/accept`, {});
  }

  decline(inviteId: string): Observable<void> {
    return this.api.post<void>(`invites/${inviteId}/decline`, {});
  }
}