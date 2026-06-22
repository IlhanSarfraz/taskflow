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
  private alertSource = new Subject<AlertMessage | null>();
  alert$ = this.alertSource.asObservable();

  show(alert: AlertMessage) {
    this.alertSource.next(alert);
  }

  success(title: string, messages: string[] = []) {
    this.show({ type: 'success', title, messages });
  }

  error(title: string, messages: string[] = []) {
    this.show({ type: 'error', title, messages });
  }

  warning(title: string, messages: string[] = []) {
    this.show({ type: 'warning', title, messages });
  }

  clear() {
    this.alertSource.next(null);
  }
}