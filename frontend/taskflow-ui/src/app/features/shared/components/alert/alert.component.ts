import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { NotificationService, AlertMessage } from '../../services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.component.html'
})
export class AlertComponent implements OnInit, OnDestroy {
  private notificationService = inject(NotificationService);
  private sub?: Subscription;
  private timer?: ReturnType<typeof setTimeout>;

  alert: AlertMessage | null = null;

  ngOnInit() {
    this.sub = this.notificationService.alert$.subscribe(alert => {
      this.alert = alert;
      clearTimeout(this.timer);

      if (alert) {
        this.timer = setTimeout(() => this.close(), 5000);
      }
    });
  }

  close() {
    this.alert = null;
    clearTimeout(this.timer);
  }

  ngOnDestroy() {
    this.sub?.unsubscribe();
    clearTimeout(this.timer);
  }
}