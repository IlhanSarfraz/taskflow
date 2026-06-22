import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { LoginRequest } from '../../models/login-request';
import { NotificationService } from '../../../shared/services/notification.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private notificationService = inject(NotificationService);
  showPassword = false;

  form = this.fb.group({
    email: [``, [Validators.required, Validators.email]],
    password: [``, [Validators.required]]
  });

  login() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      const messages: string[] = [];
      const email = this.form.get('email');
      const password = this.form.get('password');

      if (email?.hasError('required')) {
        messages.push('Email is required');
      } else if (email?.hasError('email')) {
        messages.push('Invalid email format');
      }

      if (password?.hasError('required')) {
        messages.push('Password is required');
      }

      this.notificationService.show({
        type: 'error',
        title: 'Validation Error',
        messages: messages.length ? messages : ['Please fix the form errors']
      });

      return;
    }

    const request = this.form.value as LoginRequest;

    this.authService.login(request).subscribe({
      next: () => {
        this.router.navigate(['/projects']);
      },
      error: (err) => {
        console.error('login failed', err);
        this.notificationService.show({
          type: 'error',
          title: 'Login Failed',
          messages: ['Invalid email or password. Please try again.']
        });
      }
    });
  }
}