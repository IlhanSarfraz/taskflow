import { Component, inject, signal } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AlertComponent } from "./features/shared/components/alert/alert.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AlertComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('taskflow-ui');
  private router = inject(Router);

goToMyTasks() {
  this.router.navigate(['/tasks/my']);
}
}
