import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface AlertMessage {
  type: 'success' | 'error' | 'warning';
  title: string;
  messages: string[];
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private alertSource = new Subject<AlertMessage>();

  alert$ = this.alertSource.asObservable();

  show(alert: AlertMessage) {
    this.alertSource.next(alert);
  }
}