import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router, RouterLink } from '@angular/router';
import { LoginRequest } from '../../models/login-request';

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

  form = this.fb.group({
    email: [``, [Validators.required, Validators.email]],
    password: [``, [Validators.required]]
  });

  login(){
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    const request = this.form.value as LoginRequest;
    
    this.authService.login(request).subscribe({
      next: () => {
        this.router.navigate([`/projects`]);
      },
      error: (err) =>{
        console.error(`login failed`, err);
      }
    });
  }
}
