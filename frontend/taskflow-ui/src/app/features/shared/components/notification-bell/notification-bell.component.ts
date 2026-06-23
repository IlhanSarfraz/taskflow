import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { Subscription, finalize } from 'rxjs';
import { NotificationBellService } from '../../services/notification-bell.service';
import { InvitationService } from '../../../projects/services/invitation.service';
import { AppNotification } from '../../models/notification.model';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-bell.component.html'
})
export class NotificationBellComponent implements OnInit, OnDestroy {
  private bellService = inject(NotificationBellService);
  private invitationService = inject(InvitationService);
  private toast = inject(NotificationService);
  private cdr = inject(ChangeDetectorRef);

  private sub?: Subscription;

  open = false;
  notifications: AppNotification[] = [];
  processingIds = new Set<string>();
  processingActions = new Map<string, 'accept' | 'decline'>();

  ngOnInit() {
    this.bellService.startPolling();
    this.sub = this.bellService.notifications$.subscribe(n => {
      this.notifications = n;
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
    this.bellService.stopPolling();
  }

  get unreadCount(): number {
    return this.notifications.filter(n => !n.isRead).length;
  }

  toggle() {
    this.open = !this.open;
  }

  accept(notification: AppNotification) {
      if (!notification.relatedEntityId || this.processingActions.has(notification.id)) return;

      this.processingActions.set(notification.id, 'accept');
      this.cdr.markForCheck();

      this.invitationService.accept(notification.relatedEntityId)
        .pipe(finalize(() => {
          this.processingActions.delete(notification.id);
          this.cdr.markForCheck();
        }))
        .subscribe({
          next: () => {
            this.toast.success('Invite accepted');
            this.bellService.fetchNotifications().subscribe();
          },
          error: (err) => this.toast.error(err?.error?.message ?? 'Could not accept invite')
        });
    }

  decline(notification: AppNotification) {
    if (!notification.relatedEntityId || this.processingActions.has(notification.id)) return;

    this.processingActions.set(notification.id, 'decline');
    this.cdr.markForCheck();

    this.invitationService.decline(notification.relatedEntityId)
      .pipe(finalize(() => {
        this.processingActions.delete(notification.id);
        this.cdr.markForCheck();
      }))
      .subscribe({
        next: () => {
          this.toast.success('Invite declined');
          this.bellService.fetchNotifications().subscribe();
        },
        error: (err) => this.toast.error(err?.error?.message ?? 'Could not decline invite')
      });
  }

  isAccepting(notification: AppNotification): boolean {
    return this.processingActions.get(notification.id) === 'accept';
  }

  isDeclining(notification: AppNotification): boolean {
    return this.processingActions.get(notification.id) === 'decline';
  }

  isProcessing(notification: AppNotification): boolean {
    return this.processingActions.has(notification.id);
  }

  isInvite(notification: AppNotification): boolean {
    return notification.type === 'ProjectInvite';
  }

  dismiss(notification: AppNotification) {
    if (this.processingIds.has(notification.id)) return;
    this.processingIds.add(notification.id);

    this.bellService.markRead(notification.id)
      .pipe(finalize(() => this.processingIds.delete(notification.id)))
      .subscribe(() => {
        this.bellService.fetchNotifications().subscribe();
      });
  }

  trackByNotificationId(index: number, notification: AppNotification): string {
    return notification.id;
  }
}