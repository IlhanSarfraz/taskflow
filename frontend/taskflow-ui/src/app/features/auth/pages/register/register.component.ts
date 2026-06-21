import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { RegisterRequest } from '../../models/register-request';
import { NotificationService } from '../../../shared/services/notification.service';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return password === confirmPassword ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  standalone: true,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService)
  private router = inject(Router)
  private notificationService = inject(NotificationService);

  form = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/), Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: passwordMatchValidator });

  register() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      const messages: string[] = [];

      const email = this.form.get('email');
      const password = this.form.get('password');
      const confirmPassword = this.form.get('confirmPassword');

      if (email?.hasError('required')) {
        messages.push('Email is required');
      }
      if (email?.hasError('email')) {
        messages.push('Invalid email format');
      }

      if (password?.hasError('required')) {
        messages.push('Password is required');
      } else if (password?.hasError('minlength')) {
        messages.push('Password must be at least 8 characters');
      } else if (password?.hasError('pattern')) {
        messages.push('Password must include uppercase, lowercase and number');
      }

      if (confirmPassword?.hasError('required')) {
        messages.push('Confirm password is required');
      } else if (this.form.hasError('passwordMismatch')) {
        messages.push('Passwords do not match');
      }

      this.notificationService.show({
        type: 'error',
        title: 'Validation Error',
        messages: messages.length ? messages : ['Please fix the form errors']
      });

      return;
    }
  }
}