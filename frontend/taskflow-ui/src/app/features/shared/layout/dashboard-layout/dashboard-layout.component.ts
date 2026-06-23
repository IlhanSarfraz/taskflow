import { Component, inject, OnInit, signal } from '@angular/core';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';
import { Location } from '@angular/common';
import { NotificationBellComponent } from '../../components/notification-bell/notification-bell.component';

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    NotificationBellComponent
  ],
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.scss',
})
export class DashboardLayoutComponent implements OnInit {

  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly location = inject(Location)

  collapsed = signal(false);

  userInitials = '';

  ngOnInit(): void {

    const firstName =
      localStorage.getItem('firstName') ?? '';

    const lastName =
      localStorage.getItem('lastName') ?? '';

    this.userInitials =
      `${firstName.charAt(0)}${lastName.charAt(0)}`
        .toUpperCase();
  }

  toggleSidebar() {
    this.collapsed.update(v => !v);
  }

  goBack() {
    this.location.back();
  }

  goForward() {
    this.location.forward();
  }

  logout(): void {

    this.authService.logout();

    this.router.navigate(['/login']);
  }
}