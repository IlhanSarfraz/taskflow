import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subscription, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { AppNotification } from '../models/notification.model';
import { ApiService } from '../../../core/services/api.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationBellService {
  private api = inject(ApiService);

  private notificationsSubject = new BehaviorSubject<AppNotification[]>([]);
  notifications$: Observable<AppNotification[]> = this.notificationsSubject.asObservable();

  private pollSub?: Subscription;

  startPolling(intervalMs = 20000) {
    if (this.pollSub) return;

    this.pollSub = timer(0, intervalMs)
      .pipe(switchMap(() => this.fetchNotifications()))
      .subscribe();
  }

  stopPolling() {
    this.pollSub?.unsubscribe();
    this.pollSub = undefined;
  }

  fetchNotifications() {
    return this.api.get<AppNotification[]>('notifications/my').pipe(
      switchMap(async (res) => {
        this.notificationsSubject.next(res);
        return res;
      })
    );
  }

  markRead(id: string) {
    return this.api.post<void>(`notifications/${id}/read`, {});
  }

  get unreadCount(): number {
    return this.notificationsSubject.value.filter(n => !n.isRead).length;
  }
}