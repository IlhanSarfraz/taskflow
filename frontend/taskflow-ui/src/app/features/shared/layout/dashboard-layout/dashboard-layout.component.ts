import { Component, inject, OnInit, signal } from '@angular/core';
import {
  Router,
  RouterLink,
  RouterLinkActive,
  RouterOutlet
} from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';


@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive
  ],
  templateUrl: './dashboard-layout.component.html',
  styleUrl: './dashboard-layout.component.scss',
})
export class DashboardLayoutComponent implements OnInit {

  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

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

  toggleSidebar(): void {
    this.collapsed.update(v => !v);
  }

  logout(): void {

    this.authService.logout();

    this.router.navigate(['/login']);
  }
}