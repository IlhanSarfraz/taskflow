import { Component, inject, OnInit } from '@angular/core';
import { NotificationService, AlertMessage } from '../../services/notification.service';

@Component({
  selector: 'app-alert',
  standalone: true,
  templateUrl: './alert.component.html'
})
export class AlertComponent implements OnInit {

  private notificationService = inject(NotificationService);

  alert: AlertMessage | null = null;

  ngOnInit() {
    this.notificationService.alert$
      .subscribe(alert => {
        this.alert = alert;
      });
  }

  close() {
    this.alert = null;
  }
}